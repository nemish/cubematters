using System;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Player actions")]
public class CanComplexPlayerMove : ConditionTask {

    public BBParameter<List<Transform>> childCubes;
    public BBParameter<string> direction;

    protected override string OnInit() {
        Debug.Log(String.Format("Hello! {0}", childCubes));
        return null;
    }

    protected override bool OnCheck() {
        bool canMove = true;
        foreach (Transform cube in childCubes.value) {
            CubeManager mgr = cube.GetComponent<CubeManager>();
            if (!mgr.CanMoveToDirection(direction.value)) {
                return false;
            }
        }
        return true;
        // Vector3 currentOffset = agent.transform.position - player.value.position - offset.value;
        // if ((Mathf.Abs(currentOffset.x) > offsetX.value) || (Mathf.Abs(currentOffset.y) > offsetY.value) || (Mathf.Abs(currentOffset.z) > offsetZ.value)) {
        //     return true;
        // }
        // return false;
    }
}