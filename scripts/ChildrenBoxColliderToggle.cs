using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("GameObject helpers")]
public class ChilderBoxColliderToggle : ActionTask {

    public BBParameter<bool> current = true;

    private BoxCollider[] colliders;

    protected override string OnInit() {
        colliders = agent.transform.GetComponentsInChildren<BoxCollider>();
        return null;
    }

    protected override void OnExecute() {
        foreach (BoxCollider col in colliders)
        {
            col.isTrigger = true;
        }
        EndAction(true);
    }
}