using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using NodeCanvas.Framework;


public class CubeManager : MonoBehaviour {

    public Color joinColor = new Color(0.5f, 0.4f, 0.9f);
    public Color decomposeColor = new Color(0.6f, 0.6f, 0.6f);

    private GameManager gameManager;
    private static CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
    private static TextInfo textInfo = cultureInfo.TextInfo;
    private GameObject playerCube;
    private Renderer playerCubeRenderer;
    private List<Transform> checkers = new List<Transform>();
    private bool joinEnabled = false;
    private string joinColorHex = "#D79F73";
    private Color originalColor;
    private Color emissionColor;

    private void Start() {
        playerCube = transform.Find("PlayerCube").gameObject;
        playerCubeRenderer = playerCube.GetComponent<Renderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        originalColor = playerCubeRenderer.material.GetColor("_EmissionColor");

        foreach (Transform child in transform) {
            if (child.tag == Constants.playerSenseTag) {
                checkers.Add(child);
            }
        }
    }

    public bool CanMoveToDirection(string direction) {
        SenseChecker checker = getChecker(direction);
        return checker.ObstacleOk() && !checker.IsTouchingOtherPlayer();
    }

    private SenseChecker getChecker(string direction) {
        string checkerName = getCheckerName(direction);
        return transform.Find(checkerName).GetComponent<SenseChecker>();
    }

    private bool IsStandingOnSomething() {
        SenseChecker checker = getChecker("down");
        return checker.IsTouchingPlayer() || checker.IsTouchingObstacle();
    }

    private string getCheckerName(string direction) {
        return textInfo.ToTitleCase(direction) + "Checker";
    }

    public bool IsTouchingOtherPlayCubeAnywhere() {
        foreach (Transform checker in checkers) {
            SenseChecker sense = checker.GetComponent<SenseChecker>();
            if (sense.IsTouchingOtherPlayer()) {
                return true;
            }
        }
        return false;
    }

    public List<Transform> GetPlayCubesInTouch() {
        List<Transform> cubes = new List<Transform>();
        foreach (Transform checker in checkers) {
            SenseChecker sense = checker.GetComponent<SenseChecker>();
            if (sense.IsTouchingOtherPlayer()) {
                cubes.Add(sense.GetTouchingPlayCube());
            }
        }
        return cubes;
    }

    public void EnableJoin() {
        playerCubeRenderer.material.SetColor ("_EmissionColor", joinColor);
    }

    public void DisableJoin() {
        playerCubeRenderer.material.SetColor ("_EmissionColor", originalColor);
    }

    public bool JoinEnabled() {
        return playerCubeRenderer.material.GetColor("_EmissionColor") == joinColor;
    }

    public void JoinToPlayer(Transform player) {
        transform.parent = player;
        playerCubeRenderer.material.SetColor ("_EmissionColor", originalColor);
        Graph.SendGlobalEvent("CUBE_JOINED", transform);
    }

    public void EnableDecompose() {
        playerCubeRenderer.material.SetColor ("_EmissionColor", decomposeColor);
    }

    public void DisableDecompose() {
        playerCubeRenderer.material.SetColor ("_EmissionColor", originalColor);
    }

    public bool IsOnGround() {
        return !CanMoveToDirection("down");
    }

    public bool IsInCompose() {
        return false;
    }

    public void ToDecompose() {}

    public bool CanDecompose() {
        SenseChecker upChecker = getChecker("up");
        bool topOk = true;
        if (upChecker.IsTouchingConnectedNeighbourCube()) {
            Transform cubeAbove = upChecker.GetTouchingConnectedNeighbourCube();
            topOk = cubeAbove.GetComponent<CubeManager>().IsInCompose();
        }
        return IsStandingOnSomething() && topOk;
    }
}
