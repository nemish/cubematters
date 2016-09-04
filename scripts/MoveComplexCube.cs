using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Player actions")]
public class MoveComplexCube : ActionTask {

    public BBParameter<string> direction;
    public BBParameter<float> moveTime = 0.5f;
    public BBParameter<List<Transform>> childCubes;

    private Vector3 tPos;
    private Vector3 tRot;
    private Vector3 tChildOffset;
    private Vector3 edgeChildPos;
    private Vector3 directionOffset;
    private delegate float CoordinatGetter(Transform child);
    CoordinatGetter cGetter;

    protected override void OnExecute(){

        switch (direction.value) {
            case "left":
                cGetter = (child) => -child.localPosition.x;
                directionOffset = new Vector3(-1, 0, 0);
                tRot = new Vector3(0f, 0f, 90f);
                tChildOffset = new Vector3(0f, -1f, 0f);
                break;

            case "right":
                cGetter = (child) => child.localPosition.x;
                directionOffset = new Vector3(0, 0, 0);
                tRot = new Vector3(0f, 0f, -90f);
                tChildOffset = new Vector3(1f, 0f, 0f);
                break;

            case "forward":
                cGetter = (child) => child.localPosition.z;
                directionOffset = new Vector3(0, 0, 1);
                tRot = new Vector3(90f, 0f, 0f);
                tChildOffset = new Vector3(0f, -1f, 0f);
                break;

            default:
                cGetter = (child) => -child.localPosition.z;
                directionOffset = new Vector3(0, 0, 0);
                tRot = new Vector3(-90f, 0f, 0f);
                tChildOffset = new Vector3(0f, 0f, -1f);
                break;
        }

        edgeChildPos = getEdgeChildPos();

        updateChildrenPositions();

        agent.transform.localPosition += edgeChildPos + directionOffset;

        Move();
    }

    private void updateChildrenPositions() {
        foreach (Transform child in childCubes.value) {
            child.localPosition = child.localPosition - (edgeChildPos + directionOffset);
        }
    }


    private Vector3 getEdgeChildPos() {
        List<Transform> childsOnGround = new List<Transform>();
        foreach (Transform child in childCubes.value) {
            CubeManager mgr = child.GetComponent<CubeManager>();
            if (mgr.IsOnGround()) {
                childsOnGround.Add(child);
            }
        }

        Transform edgeChild = childsOnGround[0];
        foreach (Transform child in childsOnGround) {
            if (cGetter(edgeChild) < cGetter(child)) {
                edgeChild = child;
            }
        }
        return edgeChild.localPosition;
    }

    private CoordinatGetter getCoordFn() {
        return cGetter;
    }

    private void Move() {
        Ease easeType = Ease.InOutQuart;
        Quaternion oRot = agent.transform.rotation;

        agent.transform.DORotate(tRot, moveTime.value).SetEase(easeType).OnComplete(
            () => ResetTransform(oRot)
        );
    }

    private void ResetTransform(Quaternion rot) {
        agent.transform.DetachChildren();
        agent.transform.rotation = rot;
        foreach (Transform child in childCubes.value) {
            child.rotation = rot;
            child.position += tChildOffset;
            child.parent = agent.transform;
        }
        EndAction(true);
    }
}