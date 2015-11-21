using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Player actions")]
public class IsMovingOnObstacle : ConditionTask {

    public BBParameter<string> direction;
    public BBParameter<Transform> leftChecker;
    public BBParameter<Transform> rightChecker;
    public BBParameter<Transform> forwardChecker;
    public BBParameter<Transform> backwardChecker;

    private Dictionary<string, SenseChecker> checkers = new Dictionary<string, SenseChecker>();

    protected override string OnInit() {
        checkers.Add("left", leftChecker.value.GetComponent<SenseChecker>());
        checkers.Add("right", rightChecker.value.GetComponent<SenseChecker>());
        checkers.Add("forward", forwardChecker.value.GetComponent<SenseChecker>());
        checkers.Add("backward", backwardChecker.value.GetComponent<SenseChecker>());
        return null;
    }

    protected override bool OnCheck() {
        return checkers[direction.value].IsTouchingObstacle();
    }
}
