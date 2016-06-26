using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CubeManager : MonoBehaviour {

    private Transform leftChecker;
    private Transform rightChecker;
    private Transform frontChecker;
    private Transform backChecker;

    private Dictionary<string, SenseChecker> checkers = new Dictionary<string, SenseChecker>();

    private void Start() {
        leftChecker = transform.Find("LeftChecker");
        rightChecker = transform.Find("RightChecker");
        frontChecker = transform.Find("FrontChecker");
        backChecker = transform.Find("BackChecker");
        checkers.Add("left", leftChecker.GetComponent<SenseChecker>());
        checkers.Add("right", rightChecker.GetComponent<SenseChecker>());
        checkers.Add("forward", frontChecker.GetComponent<SenseChecker>());
        checkers.Add("backward", backChecker.GetComponent<SenseChecker>());
    }

    public bool CanMoveToDirection(string direction) {
        SenseChecker checker = checkers[direction];
        return !checker.IsTouchingObstacle();
    }
}
