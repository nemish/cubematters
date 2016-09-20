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
    public BBParameter<GameObject> player1;
    public BBParameter<GameObject> player2;
    public BBParameter<List<Transform>> cubesInDecomposition;
    protected PlayerAPI api;

    protected override string OnInit(){
        // getApi = player.value.GetComponent<PlayerAPI>();
        return null;
    }

    protected PlayerAPI getApi() {
        return player.value.GetComponent<PlayerAPI>();
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
        return getApi().GetTouchingOtherPlayCubes();
    }

    protected abstract void makeAction(CubeManager mgr);
    protected virtual void begin() {}
    protected virtual void finalize() {}

}


public class EnableCubesJoin : CubesBulkAction {

    protected override List<Transform> getPlayCubes() {
        List<Transform> cubes = getApi().GetTouchingOtherPlayCubes();
        Debug.Log(string.Format("getApi transform {0} self {1}", getApi().transform, player.value));
        Debug.Log(string.Format("EnableCubesJoin {0} {1}", player.value, cubes.Count));
        // Debug.Log(string.Format("EnableCubesJoin DETAIL COUNT {0}", cubes.Where(cube => cube.GetComponent<CubeManager>().IsTouchingFreePlayCube()).ToList().Count));
        return cubes.Where(cube => cube.GetComponent<CubeManager>().IsFreeToJoin()).ToList();
    }

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
        GameObject pl = player.value;
        if (pl == null) {
            pl = mgr.GetOtherPlayCubesInTouch().First().parent.gameObject;
        }
        mgr.JoinToPlayer(player.value.transform);
    }
}


public class ResetCube : SingleCubeAction {
    protected override List<Transform> getPlayCubes() {
        if (actedCube.value == null) {
            GameObject _p = player2.value;
            if (player.value == player2.value) {
                _p = player1.value;
            }
            return _p.GetComponent<PlayerAPI>().GetChildPlayCubes();
        }
        return getSingleElementList(actedCube.value.parent.transform);
    }

    protected override void makeAction(CubeManager mgr) {
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
        detachedChilds.value = getApi().GetChildPlayCubes();
        getApi().ClearChilds();
    }

    protected override void makeAction(CubeManager mgr) {
        detachedChilds.value.Remove(mgr.transform);
        getApi().SetNewChild(mgr.transform);
        mgr.ToDefaultState();
    }

    protected override void finalize() {
        getApi().InitAPI();
        cubesInDecomposition.value.Clear();
    }
}


public abstract class DecomposeModeToggler : CubesBulkAction {
    protected override List<Transform> getPlayCubes() {
        Debug.Log(string.Format("Player  {0}", player.value));
        return player.value.transform.GetComponent<PlayerAPI>().GetChildPlayCubes().Where(
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
        List<Transform> cubes = getApi().GetChildPlayCubes().Where(isNotSuitableForComposeAlready).ToList();

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
        return getApi().GetChildPlayCubes().Where(
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



[Category("Common actions")]
public class ToChoosePlayerForDecompose : ActionTask {
    public BBParameter<GameObject> player1;
    public BBParameter<GameObject> player2;

    protected PlayerAPI api1;
    protected PlayerAPI api2;

    protected override string OnInit(){
        api1 = player1.value.GetComponent<PlayerAPI>();
        api2 = player2.value.GetComponent<PlayerAPI>();
        return null;
    }

    protected override void OnExecute(){
        api1.EnableChooseForDecomposeMode();
        api2.EnableChooseForDecomposeMode();
        EndAction(true);
    }

}


[Category("Common actions")]
public class SetPlayerForDecompose : ToChoosePlayerForDecompose {

    public BBParameter<GameObject> player;
    public BBParameter<Transform> actedCube;

    protected override void OnExecute(){
        Transform obj = actedCube.value.parent;
        Debug.Log(actedCube.value.parent);
        if (obj != null && (actedCube.value.tag == Constants.PLAYER_TAG || actedCube.value.tag == Constants.SECOND_PLAYER_TAG)) {
            player.value = obj.parent.gameObject;
            EndAction(true);
            return;
        }
        EndAction(false);
    }

}