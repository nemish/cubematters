using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


[Category("Player actions")]
public class IsMovingOnObstacle : IsMovingOnSomething {
    protected override bool checkByChecker(SenseChecker checker) {
        return checker.IsTouchingObstacle();
    }
}


[Category("Player actions")]
public class IsMovingOnEnemy : IsMovingOnSomething {
    protected override bool checkByChecker(SenseChecker checker) {
        return checker.IsTouchingEnemy();
    }
}


[Category("Player actions")]
public class IsMovingOnObstacleComplex : IsMovingOnSomething {
    protected override bool checkByChecker(SenseChecker checker) {
        return checker.IsTouchingObstacle();
    }
}


public class IsMovingOnSomething : ConditionTask {

    public BBParameter<string> direction;
    protected string[]  directions = {"left", "right", "forward", "backward"};
    public BBParameter<Transform> leftChecker;
    public BBParameter<Transform> rightChecker;
    public BBParameter<Transform> frontChecker;
    public BBParameter<Transform> backChecker;

    protected Dictionary<string, SenseChecker> checkers = new Dictionary<string, SenseChecker>();

    protected override string OnInit() {
        leftChecker = agent.transform.Find("LeftChecker");
        rightChecker = agent.transform.Find("RightChecker");
        frontChecker = agent.transform.Find("FrontChecker");
        backChecker = agent.transform.Find("BackChecker");
        checkers.Add("left", leftChecker.value.GetComponent<SenseChecker>());
        checkers.Add("right", rightChecker.value.GetComponent<SenseChecker>());
        checkers.Add("forward", frontChecker.value.GetComponent<SenseChecker>());
        checkers.Add("backward", backChecker.value.GetComponent<SenseChecker>());
        return null;
    }

    protected override bool OnCheck() {
        if (direction.value == null) {
            return checkCommon();
        }
        return checkByChecker(checkers[direction.value]);
    }

    protected virtual bool checkByChecker(SenseChecker checker) {
        return false;
    }

    protected virtual bool checkCommon() {
        return false;
    }
}