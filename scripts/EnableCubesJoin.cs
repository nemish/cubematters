using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Common actions")]
public class EnableCubesJoin : ActionTask {

    public BBParameter<GameObject> player;
    private PlayerAPI api;

    protected override string OnInit(){
        api = player.value.GetComponent<PlayerAPI>();
        return null;
    }

    protected override void OnExecute(){
        foreach (Transform child in api.GetTouchingOtherPlayCubes()) {
            CubeManager mgr = child.GetComponent<CubeManager>();
            mgr.EnableJoin();
        }
        EndAction(true);
    }
}