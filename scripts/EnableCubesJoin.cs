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
        Debug.Log("ResetCube");
        mgr.ToDefaultState();
    }
}


public class DecomposeCube : SingleCubeAction {
    protected override void makeAction(CubeManager mgr) {
        if (mgr.IsInCompose()) {
            cubesInDecomposition.value.Remove(mgr.transform);
            mgr.ToDecomposeWaiting();
        } else {
            cubesInDecomposition.value.Add(mgr.transform);
            mgr.ToDecompose();
        }
    }
}


public class DoDecompose : CubesBulkAction {
    public BBParameter<List<Transform>> detachedChilds;

    protected override List<Transform> getPlayCubes() {
        return cubesInDecomposition.value;
    }

    protected override void begin() {
        detachedChilds.value = api.GetChildPlayCubes();
        api.ClearChilds();
    }

    protected override void makeAction(CubeManager mgr) {
        detachedChilds.value.Remove(mgr.transform);
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
        if (cubesInDecomposition.value.Count > 0) {
            return mgr.IsTouchingNeighbourInCompose() && !mgr.IsInCompose();
        }
        return mgr.CanDecompose() && !mgr.IsInCompose();
    }

    protected override void makeAction(CubeManager mgr) {
        mgr.EnableDecompose();
    }

    protected override void finalize() {
        List<Transform> cubes = api.GetChildPlayCubes().Where(isNotSuitableForComposeAlready).ToList();

        foreach (Transform cube in cubes) {
            cube.GetComponent<CubeManager>().ToDefaultState();
        }
    }

    private bool isNotSuitableForComposeAlready(Transform cube) {
        CubeManager mgr = cube.GetComponent<CubeManager>();
        return !isSuitable(mgr) && !mgr.IsInCompose();
    }
}


public class DisableCubesDecompose : DecomposeModeToggler {

    public BBParameter<List<Transform>> detachedChilds;

    protected override List<Transform> getPlayCubes() {
        Debug.Log(string.Format("DisableCubesDecompose {0}", detachedChilds.value.Count));
        if (detachedChilds.value.Count > 0) {
            return detachedChilds.value;
        }
        return api.GetChildPlayCubes().Where(
            cube => isSuitable(cube.GetComponent<CubeManager>())
        ).ToList();
    }

    protected override bool isSuitable(CubeManager mgr) {
        return mgr.IsInCompose() || mgr.IsWaitingForDecompose();
    }

    protected override void begin() {
        cubesInDecomposition.value.Clear();
    }

    protected override void makeAction(CubeManager mgr) {
        Debug.Log(string.Format("DisableCubesDecompose {0}", mgr.transform.name));
        mgr.DisableDecompose();
    }

    protected override void finalize() {
        detachedChilds.value.Clear();
    }
}
