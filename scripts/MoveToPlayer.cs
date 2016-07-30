using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using DG.Tweening;

[Category("MainCamera actions")]
public class MoveToPlayer : ActionTask {

    public BBParameter<Transform> cameraAim;
    public BBParameter<float> moveTime = 2f;

    private CameraAim cameraAimAPI;

    protected override string OnInit() {
        if (cameraAim.value == null) {
            return "Player are not specified";
        }
        cameraAimAPI = cameraAim.value.GetComponent<CameraAim>();
        return null;
    }

    protected override void OnUpdate(){
        Vector3 targetPos = cameraAimAPI.getCamPosition() + Vector3.back * 2;
        agent.transform.position = Vector3.Lerp(agent.transform.position, targetPos, Time.deltaTime);

        if (Vector3.Distance(agent.transform.position, targetPos) < 0.1f) {
            EndAction(true);
        }
    }

}
