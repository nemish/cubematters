using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;


[Category("Player actions")]
public class IsMovingOnObstacle : IsMovingOnSomething {
    protected override bool check(SenseChecker checker) {
        return checker.IsTouchingObstacle();
    }
}


[Category("Player actions")]
public class IsMovingOnEnemy : IsMovingOnSomething {
    protected override bool check(SenseChecker checker) {
        return checker.IsTouchingEnemy();
    }
}


[Category("Player actions")]
public class IsMovingOnObstacleComplex : IsMovingOnSomething {
    protected override bool check(SenseChecker checker) {
        return checker.IsTouchingObstacle();
    }
}


public class IsMovingOnSomething : ConditionTask {

    public BBParameter<string> direction;
    public BBParameter<Transform> leftChecker;
    public BBParameter<Transform> rightChecker;
    public BBParameter<Transform> frontChecker;
    public BBParameter<Transform> backChecker;

    private Dictionary<string, SenseChecker> checkers = new Dictionary<string, SenseChecker>();

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
        return check(checkers[direction.value]);
    }

    protected virtual bool check(SenseChecker checker) {
        return false;
    }
}