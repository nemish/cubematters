using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;


public class CubeManager : MonoBehaviour {

    private static CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
    private static TextInfo textInfo = cultureInfo.TextInfo;
    private GameObject playerCube;
    private List<Transform> checkers = new List<Transform>();

    private void Start() {
        playerCube = transform.Find("PlayerCube").transform;
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
        Debug.Log("EnableJoin");
        playerCube.renderer.material.SetFloat("_Shininess", 0.4);
    }

    public bool IsOnGround() {
        return !CanMoveToDirection("down");
    }
}
