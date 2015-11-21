using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Enemy actions")]
public class MoveToNextPoint : ActionTask {

    public BBParameter<List<Transform>> points;
    public BBParameter<float> moveTime = 1.5f;
    public BBParameter<string> changePointType = "sequence";
    public BBParameter<int> targetPointIndex = 0;
    public BBParameter<int> currentPointIndex;

    delegate void UpdateNextPoint();
    UpdateNextPoint updateNextPoint;

    protected override string OnInit() {
        if (changePointType.value == "sequence") {
            updateNextPoint = nextPointSequenced;
        } else if (changePointType.value == "reverse") {
            updateNextPoint = nextPointReversed;
        }
        return null;
    }

    protected override void OnExecute() {
        Transform targetPoint = points.value[targetPointIndex.value];
        agent.transform.DOMove(targetPoint.position, moveTime.value).OnComplete(EndMove);
    }

    private void nextPointSequenced() {
        if (++targetPointIndex.value == points.value.Count) {
            targetPointIndex.value = 0;
        }
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
    }

    private void EndMove() {
        updateNextPoint();
        EndAction(true);
    }
}
