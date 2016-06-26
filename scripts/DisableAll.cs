using UnityEngine;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Player actions")]
public class DisableAll : ActionTask {

    protected override void OnExecute(){
        DeathSpawner.SetActiveRecursively(agent.gameObject, false);
        EndAction(true);
    }
}