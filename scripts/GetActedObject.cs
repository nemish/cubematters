using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Common actions")]
public class GetActedObject : ActionTask {

    public BBParameter<string> hitLayer = null;
    public BBParameter<Transform> actedObject;

    protected override void OnExecute() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        actedObject.value = null;

        int layerMask = Physics.DefaultRaycastLayers;
        if (hitLayer.value != null) {
            layerMask = LayerMask.GetMask(hitLayer.value);
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) {
            Transform tr = hit.collider.transform;
            if (tr.parent.tag == Constants.PLAYER_CUBE_CONTAINER_TAG) {
                tr = tr.parent;
            }
            if (tr.tag == Constants.PLAYER_CUBE_CONTAINER_TAG) {
                CubeManager mgr = tr.GetComponent<CubeManager>();
                if (mgr.JoinEnabled()) {
                    actedObject.value = hit.collider.transform;
                }
            }
        }

        EndAction(true);
    }

}
