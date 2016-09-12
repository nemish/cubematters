using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using NodeCanvas.Framework;


public class CubeManager : MonoBehaviour {

    public Color joinColor = new Color(0.5f, 0.7f, 1.0f);
    public Color waitForDecmoposeColor = new Color(0.6f, 0.6f, 0.6f);
    public Color inDecomposeColor = new Color(0.3f, 0.6f, 0.14f);
    public Color potentialPlayerColor = new Color(0.35f, 0.35f, 0.35f);
    public Color playerColor = new Color(0.61f, 0.41f, 0.26f);
    public Color secondPlayerColor = new Color(0.3f, 0.21f, 0.86f);

    private GameManager gameManager;
    private static CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
    private static TextInfo textInfo = cultureInfo.TextInfo;
    private GameObject playerCube;
    private Renderer playerCubeRenderer;
    private List<Transform> checkers = new List<Transform>();
    private bool joinEnabled = false;
    private string joinColorHex = "#D79F73";
    private Color originalColor;
    private Transform centerPoint;

    private delegate bool CheckSense(SenseChecker sense);
    CheckSense checkSense;

    private void Start() {
        playerCube = transform.Find("PlayerCube").gameObject;
        centerPoint = transform.Find("CubeCenterPoint");
        playerCubeRenderer = playerCube.GetComponent<Renderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        originalColor = getOriginalColor();
        setColor(originalColor);

        foreach (Transform child in transform) {
            if (child.tag == Constants.playerSenseTag) {
                checkers.Add(child);
            }
        }
    }

    private void Update() {
        if (!isDefaultState() && !IsTouchingOtherPlayCubeAnywhere() && !IsWaitingForDecompose() && !IsInCompose()) {
            ToDefaultState();
        }
    }

    public Vector3 GetCenterPosition() {
        return centerPoint.position;
    }

    private bool isDefaultState() {
        return getCurrentColor() == getOriginalColor();
    }

    public bool CanMoveToDirection(string direction) {
        SenseChecker checker = getChecker(direction);
        return checker.ObstacleOk() && !checker.IsTouchingOtherPlayer();
    }

    private SenseChecker getChecker(string direction) {
        string checkerName = getCheckerName(direction);
        return transform.Find(checkerName).GetComponent<SenseChecker>();
    }

    private Color getOriginalColor() {
        Color c = potentialPlayerColor;
        if (transform.parent && transform.parent.tag == Constants.PLAYER_TAG) {
            c = playerColor;
        } else if (transform.parent && transform.parent.tag == Constants.SECOND_PLAYER_TAG) {
            c = secondPlayerColor;
        } else if (IsTouchingOtherPlayCubeAnywhere()) {
            c = joinColor;
        }
        return c;
    }

    public bool IsStandingOnSomething() {
        SenseChecker checker = getChecker("down");
        return checker.IsTouchingPlayer() || checker.IsTouchingObstacle();
    }

    public bool IsStandingOnSomethingExternal() {
        SenseChecker checker = getChecker("down");
        return checker.IsTouchingOtherPlayer() || checker.IsTouchingObstacle();
    }

    private string getCheckerName(string direction) {
        return textInfo.ToTitleCase(direction) + "Checker";
    }

    public bool IsTouchingOtherPlayCubeAnywhere() {
        return isAnySenseSuits(sense => sense.IsTouchingOtherPlayer());
    }

    public bool IsTouchingNeighbourCubeAnywhere() {
        return isAnySenseSuits(sense => sense.IsTouchingNeighbourCube());
    }

    private bool isAnySenseSuits(CheckSense checkFn) {
        foreach (Transform checker in checkers) {
            SenseChecker sense = checker.GetComponent<SenseChecker>();
            if (checkFn(sense)) {
                return true;
            }
        }
        return false;
    }

    public List<Transform> GetOtherPlayCubesInTouch() {
        return getPlayCubesByChecker(sense => sense.IsTouchingOtherPlayer());
    }

    public List<Transform> GetNeighbourPlayCubesInTouch() {
        return getPlayCubesByChecker(sense => sense.IsTouchingNeighbourCube());
    }

    private List<Transform> getPlayCubesByChecker(CheckSense checkFn) {
        List<Transform> cubes = new List<Transform>();
        foreach (Transform checker in checkers) {
            SenseChecker sense = checker.GetComponent<SenseChecker>();
            if (checkFn(sense)) {
                cubes.Add(sense.GetTouchingPlayCube());
            }
        }
        return cubes;
    }

    public void EnableJoin() {
        setColor(joinColor);
    }

    public void ToDefaultState() {
        Debug.Log(transform.name);
        originalColor = getOriginalColor();
        setColor(originalColor);
    }

    public void DisableJoin() {
        Debug.Log(string.Format("DisableJoin"));
        ToDefaultState();
    }

    public bool JoinEnabled() {
        return getCurrentColor() == joinColor;
    }

    public void JoinToPlayer(Transform player) {
        transform.parent = player;
        ToDefaultState();
        Graph.SendGlobalEvent("CUBE_JOINED", transform);
    }

    public void EnableDecompose() {
        setColor(waitForDecmoposeColor);
    }

    public void DisableDecompose() {
        ToDefaultState();
    }

    public bool IsOnGround() {
        return !CanMoveToDirection("down");
    }

    public bool IsInCompose() {
        return getCurrentColor() == inDecomposeColor;
    }

    public void ToDecompose() {
        setColor(inDecomposeColor);
    }

    public bool IsTouchingNeighbourInCompose() {
        foreach (Transform cube in GetNeighbourPlayCubesInTouch()) {
            CubeManager mgr = cube.GetComponent<CubeManager>();
            if (mgr.IsInCompose()) {
                return true;
            }
        }
        return false;
    }

    public void ToDecomposeWaiting() {
        setColor(waitForDecmoposeColor);
    }

    public bool IsWaitingForDecompose() {
        return getCurrentColor() == waitForDecmoposeColor;
    }

    private void setColor(Color color) {
        playerCubeRenderer.material.SetColor ("_EmissionColor", color);
    }

    private Color getCurrentColor() {
        return playerCubeRenderer.material.GetColor("_EmissionColor");
    }

    public bool CanDecompose() {
        SenseChecker upChecker = getChecker("up");
        bool topOk = true;
        Transform cubeAbove = upChecker.GetTouchingConnectedNeighbourCube();
        if (cubeAbove != null) {
            topOk = cubeAbove.GetComponent<CubeManager>().IsInCompose();
        }
        return IsStandingOnSomething() && topOk;
    }
}
