using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using DG.Tweening;


[Category("MainCamera actions")]
public class BaseCameraTask : ActionTask {
    public BBParameter<Quaternion> targetRotation;
    public BBParameter<Vector3> offsetDirection = Vector3.back;
    public BBParameter<float> moveTime = 2f;
    public BBParameter<float> offset = 2f;
    public BBParameter<float> angle = 70f;
}

[Category("MainCamera actions")]
public class MoveToPlayer : BaseCameraTask {

    public BBParameter<Transform> cameraAim;

    private CameraAim cameraAimAPI;

    protected override string OnInit() {
        if (cameraAim.value != null) {
            cameraAimAPI = cameraAim.value.GetComponent<CameraAim>();
        }
        targetRotation.value = agent.transform.rotation;
        return null;
    }

    protected override void OnUpdate(){
        Vector3 targetPos = cameraAimAPI.getCamPosition() + offsetDirection.value * offset.value;
        agent.transform.position = Vector3.Lerp(agent.transform.position, targetPos, Time.deltaTime);
        agent.transform.rotation = Quaternion.Lerp(agent.transform.rotation, targetRotation.value, Time.deltaTime);

        if (Vector3.Distance(agent.transform.position, targetPos) < 0.1f) {
            EndAction(true);
        }
    }
}


[Category("MainCamera actions")]
public class SetDecomposeCamPosition : BaseCameraTask {

    public BBParameter<string> direction;

    private float rotationDelta;
    private Vector3 lastSide = Vector3.back;
    private string currentPos;

    protected override void OnExecute() {
        float rotationDelta = 0f;
        switch (direction.value) {
            case "left":
                offsetDirection.value = getNextDirectionVector(Vector3.left);
                break;

            default:
                offsetDirection.value = getNextDirectionVector(Vector3.right);
                break;
        }

        Debug.Log(offsetDirection.value);

        switch ((int)offsetDirection.value.x) {
            case 1:
                rotationDelta = 270f;
                break;
            case -1:
                rotationDelta = 90f;
                break;
        }

        switch ((int)offsetDirection.value.z) {
            case 1:
                rotationDelta = 180f;
                break;
            case -1:
                rotationDelta = 0f;
                break;
        }

        Transform cam = agent.transform;
        targetRotation.value = Quaternion.Euler(angle.value, rotationDelta, 0f);
        EndAction(true);
    }


    private Vector3 getNextDirectionVector(Vector3 side) {
        if (lastSide == Vector3.left && side == Vector3.left) {
            lastSide = Vector3.forward;
        } else if (lastSide == Vector3.right && side == Vector3.left) {
            lastSide = Vector3.back;
        } else if (lastSide == Vector3.forward && side == Vector3.left) {
            lastSide = Vector3.right;
        } else if (lastSide == Vector3.back && side == Vector3.left) {
            lastSide = Vector3.left;
        }

        if (lastSide == Vector3.left && side == Vector3.right) {
            lastSide = Vector3.back;
        } else if (lastSide == Vector3.right && side == Vector3.right) {
            lastSide = Vector3.forward;
        } else if (lastSide == Vector3.forward && side == Vector3.right) {
            lastSide = Vector3.left;
        } else if (lastSide == Vector3.back && side == Vector3.right) {
            lastSide = Vector3.right;
        }
        return lastSide;
    }

}


[Category("MainCamera actions")]
public class ResetDecomposeCamPosition : SetDecomposeCamPosition {

    protected override void OnExecute() {
        offsetDirection.value = Vector3.back;
        targetRotation.value = Quaternion.Euler(70f, 0f, 0f);
        EndAction(true);
    }

}