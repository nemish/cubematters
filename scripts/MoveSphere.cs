using UnityEngine;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Player actions")]
public class MoveSphere : ActionTask {

    public BBParameter<Vector3> direction = Vector3.zero;
    public BBParameter<float> moveSpeed  = 10f;

    protected override void OnExecute(){
        if (direction.value != Vector3.zero) {
            agent.GetComponent<Rigidbody>().AddForce(direction.value * moveSpeed.value);
        }
        EndAction(true);
    }
}