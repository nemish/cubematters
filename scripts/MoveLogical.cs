using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;


[Category("Movement")]
public class MoveLogical : ActionTask {

    public BBParameter<float> moveTime = 1.5f;
    public BBParameter<int> primaryStep = 2;
    public BBParameter<int> oppositeStep = 1;

    public enum MoveType {Line, Sequence};
    public BBParameter<MoveType> moveType = MoveType.Line;

    public MoveDirection.Direction initialDirection = MoveDirection.Direction.Left;

    public struct MoveInfo {
        public MoveDirection.Direction dir;
        public int step;
    }

    public List<MoveInfo> directedSteps = new List<MoveInfo>();

    private IMoveUtils moveUtils;

    private Vector3 nextPoint;
    private MoveDirection direction;

    private ObstacleChecker[] checkers;

    private int currentStep;

    protected override string OnInit() {
        checkers = agent.transform.GetComponentsInChildren<ObstacleChecker>();
        if (moveType.value == MoveType.Line) {
            moveUtils = new MoveUtilsSimpleLine(initialDirection, primaryStep.value, oppositeStep.value);
        } else if (moveType.value == MoveType.Sequence) {
            moveUtils = new MoveUtilsSeqence(directedSteps);
        }
        direction = moveUtils.InitMoveDirection();
        updateNextPoint();
        return null;
    }

    protected override void OnExecute() {
        move();
    }

    private void updateNextPoint() {
        currentStep = moveUtils.GetCurrentStep();

        Vector3 nextPointOffsetVector = direction.AsVector() * currentStep;
        nextPoint = agent.transform.position + nextPointOffsetVector;

        if (willCollideWithObstacleOnMove()) {
            currentStep = (int) getStepCorrectedByObstacle();
            nextPoint = agent.transform.position + currentStep * direction.AsVector();
            direction = moveUtils.HandleObstacle(currentStep);
            fillPathVolume();
            return;
        }
        fillPathVolume();

        direction = moveUtils.GetNextDirection();

    }

    private void move () {
        agent.transform.DOMove(nextPoint, moveTime.value).OnComplete(EndMove);
    }

    private void EndMove() {
        updateNextPoint();
        EndAction(true);
    }

    private bool willCollideWithObstacleOnMove() {
        foreach (ObstacleChecker checker in checkers) {
            if (checker.WillCollide(direction, currentStep)) {
                return true;
            }
        }
        return false;
    }

    private void fillPathVolume() {
        foreach (ObstacleChecker checker in checkers) {
            checker.FillPathVolume(direction, currentStep);
        }
    }

    private float getStepCorrectedByObstacle() {
        float step = checkers.First().GetObstacleOffsetStep(direction, currentStep);
        foreach (ObstacleChecker checker in checkers.Skip(1)) {
            float s = checker.GetObstacleOffsetStep(direction, currentStep);

            if (s < step) {
                step = s;
            }
        }
        return step;
    }
}


interface IMoveUtils {
    int GetCurrentStep();
    MoveDirection GetNextDirection();
    MoveDirection InitMoveDirection();
    MoveDirection HandleObstacle(int step);
}


class MoveUtilsSimpleLine : IMoveUtils {

    private MoveDirection direction;
    private int primaryStep;
    private int oppositeStep;

    public MoveUtilsSimpleLine(MoveDirection.Direction initDir, int pr_st, int op_st) {
        direction = new MoveDirection(initDir);
        primaryStep = pr_st;
        oppositeStep = op_st;
    }

    public int GetCurrentStep() {
        int step = oppositeStep;
        if (direction.IsPrimary()) {
            step = primaryStep;
        }
        return step;
    }

    public MoveDirection GetNextDirection() {
        direction.SetOpposite();
        return direction;
    }

    public MoveDirection InitMoveDirection() {
        return direction;
    }

    public MoveDirection HandleObstacle(int step) {
        direction.Invert();
        return direction;
    }
}


class MoveUtilsSeqence : IMoveUtils {

    private List<MoveLogical.MoveInfo> directedSteps = new List<MoveLogical.MoveInfo>();
    private int nextIndex = 0;
    private int stepOffset = 0;

    private delegate MoveDirection GetDirection();
    private GetDirection getDirection;

    public MoveUtilsSeqence(List<MoveLogical.MoveInfo> st) {
        directedSteps = st;
        getDirection = GetNext;
    }

    public int GetCurrentStep() {
        int step = directedSteps[nextIndex].step - stepOffset;
        stepOffset = 0;
        return step;
    }

    public MoveDirection GetNextDirection() {
        return getDirection();
    }

    public MoveDirection InitMoveDirection() {
        return getCurrentDirection();
    }

    public MoveDirection HandleObstacle(int stoppedStepLenght) {
        getDirection = invertDirectionChangingDir();
        stepOffset = GetCurrentStep() - stoppedStepLenght;
        return getDir();
    }

    private MoveDirection getCurrentDirection() {
        return new MoveDirection(directedSteps[nextIndex].dir);
    }

    private MoveDirection GetNext() {
        if (nextIndex == directedSteps.Count - 1) {
            getDirection = GetPrevious;
            return getDir();
        }
        ++nextIndex;
        return getDir();
    }

    private MoveDirection GetPrevious() {
        if (nextIndex == 0) {
            getDirection = GetNext;
            return getDir();
        }
        --nextIndex;
        return getDir();
    }

    private MoveDirection getOppositeDirection() {
        MoveDirection curDir = getCurrentDirection();
        curDir.SetOpposite();
        return curDir;
    }

    private GetDirection invertDirectionChangingDir() {
        if (getDirection == GetNext) {
            return GetPrevious;
        }
        return GetNext;
    }

    private MoveDirection getDir() {
        if (getDirection == GetPrevious) {
            MoveDirection curDir = getCurrentDirection();
            curDir.SetOpposite();
            return curDir;
        } else {
            return getCurrentDirection();
        }
    }

}