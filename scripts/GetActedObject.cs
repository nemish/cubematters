using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Common actions")]
public class GetActedObject : ActionTask {

    public BBParameter<string> hitLayer = null;
    public BBParameter<Transform> actedObject;
    public BBParameter<bool> decomposing;

    private delegate bool ActionChecker(CubeManager mgr);
    private ActionChecker actionCheckerFn;

    protected override void OnExecute() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        actedObject.value = null;
        actionCheckerFn = CheckJoin;
        string objectTag = Constants.PLAYER_CUBE_CONTAINER_TAG;
        if (decomposing.value) {
            actionCheckerFn = CheckCompose;
        }

        int layerMask = Physics.DefaultRaycastLayers;
        if (hitLayer.value != null) {
            layerMask = LayerMask.GetMask(hitLayer.value);
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            Transform tr = hit.collider.transform;
            if (tr.parent.tag == objectTag) {
                tr = tr.parent;
            }
            if (tr.tag == objectTag) {
                CubeManager mgr = tr.GetComponent<CubeManager>();
                if (actionCheckerFn(mgr)) {
                    actedObject.value = hit.collider.transform;
                }
            }
        }

        EndAction(true);
    }

    private bool CheckJoin(CubeManager mgr) {
        return mgr.JoinEnabled();
    }

    private bool CheckCompose(CubeManager mgr) {
        return mgr.IsWaitingForDecompose();
    }

}
