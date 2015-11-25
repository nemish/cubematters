using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Common actions")]
public class FollowTarget : ActionTask {

    public BBParameter<Transform> target;

    private Vector3 offset;

    protected override string OnInit() {
        offset = agent.transform.position - target.value.position;
        return null;
    }

    protected override void OnUpdate() {
        agent.transform.position = target.value.position + offset;
    }

}
