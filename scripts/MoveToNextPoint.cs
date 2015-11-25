using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Movement")]
public class MoveToNextPoint : ActionTask {

    public BBParameter<List<Transform>> points;
    public BBParameter<float> moveTime = 1.5f;
    public BBParameter<string> changePointType = "sequence";
    public BBParameter<string> endEvent;
    public BBParameter<int> targetPointIndex = 0;
    public BBParameter<int> currentPointIndex;
    public BBParameter<bool> finished = false;

    delegate void UpdateNextPoint();
    UpdateNextPoint updateNextPoint;

    protected override string OnInit() {
        if (changePointType.value == "sequence") {
            updateNextPoint = nextPointSequenced;
        } else if (changePointType.value == "reverse") {
            updateNextPoint = nextPointReversed;
        } else if (changePointType.value == "stop_on_end") {
            updateNextPoint = nextPointStopOnEnd;
        }
        return null;
    }

    protected override void OnExecute() {
        move();
    }

    private void nextPointSequenced() {
        if (++targetPointIndex.value == points.value.Count) {
            targetPointIndex.value = 0;
        }
        EndAction(true);
    }

    private void move () {
        Transform targetPoint = points.value[targetPointIndex.value];
        agent.transform.DOMove(targetPoint.position, moveTime.value).OnComplete(EndMove);
    }

    private void nextPointReversed() {
        if (currentPointIndex.value < targetPointIndex.value) {
            currentPointIndex.value = targetPointIndex.value;
            if (targetPointIndex.value < points.value.Count - 1) {
                ++targetPointIndex.value;
            } else {
                --targetPointIndex.value;
            }
        } else {
            currentPointIndex.value = targetPointIndex.value;
            if (targetPointIndex.value > 0) {
                --targetPointIndex.value;
            } else {
                ++targetPointIndex.value;
            }
        }
        EndAction(true);
    }

    private void nextPointStopOnEnd() {
        if (++targetPointIndex.value == points.value.Count) {
            finished.value = true;
            if (endEvent.value != null) {
                Graph.SendGlobalEvent(endEvent.value);
            }
            EndAction(true);
        }
        move();
    }

    private void EndMove() {
        updateNextPoint();
    }
}
