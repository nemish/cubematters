using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Player actions")]
public class CanComplexPlayerMove : ConditionTask {

    public BBParameter<Transform> player;
    public BBParameter<List<Transform>> childCubes;
    public BBParameter<string> direction;

    protected PlayerAPI api;

    protected override string OnInit() {
        if (player.value != null) {
            api = player.value.GetComponent<PlayerAPI>();
        } else {
            api = agent.transform.GetComponent<PlayerAPI>();
        }
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
public class IsTouchingOtherFreePlayCube : CanComplexPlayerMove {

    protected override bool OnCheck() {
        bool check = false;
        foreach (Transform cube in childCubes.value) {
            CubeManager mgr = cube.GetComponent<CubeManager>();
            Debug.Log(string.Format("IsTouchingOtherFreePlayCube {0}", mgr.transform.name));
            if (mgr.IsTouchingFreePlayCube()) {
                check = true;
                break;
            }
        }
        return check;
    }
}



[Category("Player actions")]
public class IsStandingOnSomething : CanComplexPlayerMove {

    protected override bool OnCheck() {
        return childCubes.value.Where(
            cube => cube.GetComponent<CubeManager>().IsStandingOnSomethingExternal()
        ).ToList().Any();
    }
}
