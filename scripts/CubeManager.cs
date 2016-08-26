using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using NodeCanvas.Framework;


public class CubeManager : MonoBehaviour {

    private static CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
    private static TextInfo textInfo = cultureInfo.TextInfo;
    private GameObject playerCube;
    private Renderer playerCubeRenderer;
    private List<Transform> checkers = new List<Transform>();
    private bool joinEnabled = false;
    private string joinColorHex = "#D79F73";
    private Color originalColor;
    private Color joinColor = new Color(0.9f, 0.6f, 0.4f);
    private Color emissionColor;

    private void Start() {
        playerCube = transform.Find("PlayerCube").gameObject;
        playerCubeRenderer = playerCube.GetComponent<Renderer>();

        // emissionColor = playerCubeRenderer.material.GetColor("_EmissionColor");
        originalColor = playerCubeRenderer.material.GetColor("_EmissionColor");
        // Color.TryParseHexString("#F00", out joinColor);

        foreach (Transform child in transform) {
            if (child.tag == Constants.playerSenseTag) {
                checkers.Add(child);
            }
        }
    }

    public bool CanMoveToDirection(string direction) {
        string checkerName = textInfo.ToTitleCase(direction) + "Checker";
        SenseChecker checker = transform.Find(checkerName).GetComponent<SenseChecker>();
        return checker.ObstacleOk() && !checker.IsTouchingOtherPlayer();
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
        Debug.Log("EnableDecompose");
    }

    public void DisableDecompose() {
        Debug.Log("DisableDecompose");
    }

    public bool IsOnGround() {
        return !CanMoveToDirection("down");
    }
}
