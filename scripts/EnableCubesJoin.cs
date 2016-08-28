using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;


[Category("Common actions")]
public abstract class CubesBulkAction : ActionTask {
    public BBParameter<GameObject> player;
    public BBParameter<List<Transform>> cubesInDecomposition;
    protected PlayerAPI api;

    protected override string OnInit(){
        api = player.value.GetComponent<PlayerAPI>();
        return null;
    }

    protected override void OnExecute(){
        foreach (Transform child in getPlayCubes()) {
            CubeManager mgr = child.GetComponent<CubeManager>();
            makeAction(mgr);
        }
        EndAction(true);
    }

    protected virtual List<Transform> getPlayCubes() {
        return api.GetTouchingOtherPlayCubes();
    }

    protected abstract void makeAction(CubeManager mgr);

}


public class EnableCubesJoin : CubesBulkAction {

    protected override void makeAction(CubeManager mgr) {
        mgr.EnableJoin();
    }
}


public class DisableCubesJoin : CubesBulkAction {

    protected override void makeAction(CubeManager mgr) {
        mgr.DisableJoin();
    }
}


public abstract class SingleCubeAction : CubesBulkAction {
    public BBParameter<Transform> actedCube;

    protected override List<Transform> getPlayCubes() {
        return new List<Transform>(new Transform[] {actedCube.value.parent.transform});
    }
}


public class JoinCubes : SingleCubeAction {
    protected override void makeAction(CubeManager mgr) {
        mgr.JoinToPlayer(player.value.transform);
    }
}


public class DecomposeCube : SingleCubeAction {
    protected override void makeAction(CubeManager mgr) {
        cubesInDecomposition.value.Add(mgr.transform);
        // mgr.ToDecompose();
    }
}


public abstract class DecomposeAction : CubesBulkAction {
    protected override List<Transform> getPlayCubes() {
        return api.GetChildPlayCubes().Where(
            cube => cube.GetComponent<CubeManager>().CanDecompose()
        ).ToList();
    }
}


public class EnableCubesDecompose : DecomposeAction {

    protected override void makeAction(CubeManager mgr) {
        mgr.EnableDecompose();
    }
}


public class DisableCubesDecompose : DecomposeAction {

    protected override void makeAction(CubeManager mgr) {
        cubesInDecomposition.value.Clear();
        mgr.DisableDecompose();
    }
}