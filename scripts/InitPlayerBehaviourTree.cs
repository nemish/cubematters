using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using DG.Tweening;

[Category("Player actions")]
public class InitPlayerBehaviourTree : ActionTask {

    public BBParameter<List<Transform>> childCubes;

    protected override void OnExecute(){
        PlayerAPI api = agent.transform.GetComponent<PlayerAPI>();
        api.InitAPI();
        childCubes.value = api.GetChildPlayCubes();
        EndAction(true);
    }

}
