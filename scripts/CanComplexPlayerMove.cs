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
        return null;
    }

    protected override bool OnCheck() {
        bool check = true;
        foreach (Transform cube in childCubes.value) {
            CubeManager mgr = cube.GetComponent<CubeManager>();
            if (!mgr.CanMoveToDirection(direction.value)) {
                check = false;
                break;
            }
        }
        return check;
    }
}



[Category("Player actions")]
public class IsTouchingOtherPlayCube : CanComplexPlayerMove {

    protected override bool OnCheck() {
        bool check = false;
        foreach (Transform cube in childCubes.value) {
            CubeManager mgr = cube.GetComponent<CubeManager>();
            if (mgr.IsTouchingOtherPlayCubeAnywhere()) {
                check = true;
                break;
            }
        }
        return check;
    }
}