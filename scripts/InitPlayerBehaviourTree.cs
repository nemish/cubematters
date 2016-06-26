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
        foreach (Transform child in agent.transform) {
            if (child.tag == "Player") {
                childCubes.value.Add(child);
            }
        }
        EndAction(true);
    }

}
