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
        begin();
        foreach (Transform child in getPlayCubes()) {
            CubeManager mgr = child.GetComponent<CubeManager>();
            makeAction(mgr);
        }
        finalize();
        EndAction(true);
    }

    protected virtual List<Transform> getPlayCubes() {
        return api.GetTouchingOtherPlayCubes();
    }

    protected abstract void makeAction(CubeManager mgr);
    protected virtual void begin() {}
    protected virtual void finalize() {}

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
        return getSingleElementList(actedCube.value.parent.transform);
    }

    protected virtual List<Transform> getSingleElementList(Transform cube) {
        return new List<Transform>(new Transform[] {cube});
    }
}


public class JoinCubes : SingleCubeAction {
    protected override void makeAction(CubeManager mgr) {
        mgr.JoinToPlayer(player.value.transform);
    }
}


public class ResetCube : SingleCubeAction {
    protected override void makeAction(CubeManager mgr) {
        mgr.ToDefaultState();
    }
}


public class DecomposeCube : SingleCubeAction {
    protected override void makeAction(CubeManager mgr) {
        Debug.Log("DECOMPOSE ACTION BEGIN!!!");
        Debug.Log(mgr.transform.name);
        if (mgr.IsInCompose()) {
            Debug.Log("NOT IsInCompose!!!!!!!!!");
            cubesInDecomposition.value.Remove(mgr.transform);
            mgr.ToDecomposeWaiting();
        } else {
            Debug.Log(cubesInDecomposition.value);
            Debug.Log("IsInCompose");
            cubesInDecomposition.value.Add(mgr.transform);
            mgr.ToDecompose();
            Debug.Log(cubesInDecomposition.value);
        }
        Debug.Log(cubesInDecomposition.value);
    }
}


public class DoDecompose : CubesBulkAction {
    protected override List<Transform> getPlayCubes() {
        return cubesInDecomposition.value;
    }

    protected override void begin() {
        api.ClearChilds();
    }

    protected override void makeAction(CubeManager mgr) {
        api.SetNewChild(mgr.transform);
        mgr.ToDefaultState();
    }

    protected override void finalize() {
        api.InitAPI();
        cubesInDecomposition.value.Clear();
    }
}


public abstract class DecomposeModeToggler : CubesBulkAction {
    protected override List<Transform> getPlayCubes() {
        return api.GetChildPlayCubes().Where(
            cube => isSuitable(cube.GetComponent<CubeManager>())
        ).ToList();
    }

    protected virtual bool isSuitable(CubeManager mgr) {
        return mgr.CanDecompose();
    }
}


public class EnableCubesDecompose : DecomposeModeToggler {

    protected override bool isSuitable(CubeManager mgr) {
        return mgr.CanDecompose() && !mgr.IsInCompose();
    }

    protected override void makeAction(CubeManager mgr) {
        mgr.EnableDecompose();
    }
}


public class DisableCubesDecompose : DecomposeModeToggler {

    protected override void makeAction(CubeManager mgr) {
        cubesInDecomposition.value.Clear();
        mgr.DisableDecompose();
    }
}