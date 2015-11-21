using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using DG.Tweening;

[Category("MainCamera actions")]
public class MoveToPlayer : ActionTask {

    public BBParameter<Transform> player;
    public BBParameter<float> moveTime = 2f;
    public BBParameter<Vector3> offset;

    protected override string OnInit() {
        if (player.value == null || offset.value == null) {
            return "Player or offset are not specified";
        }
        return null;
    }


    protected override void OnUpdate(){
        agent.transform.position = Vector3.Lerp(agent.transform.position, player.value.position + offset.value, Time.deltaTime);

        if (Vector3.Distance(agent.transform.position - player.value.position, offset.value) < 0.1f) {
            EndAction(true);
        }
    }

}
