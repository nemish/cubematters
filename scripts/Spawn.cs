using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;


[Category("Utils")]
public class Spawn : ActionTask {

    public BBParameter<GameObject> enemy;
    public BBParameter<int> count = 3;

    private int yPos = 1;
    private int currentOffsetX = 0;
    private int currentOffsetZ = 10;

    protected override string OnInit() {
        if (enemy.value == null) {
            EndAction(true);
        }
        return null;
    }

    protected override void OnExecute() {
        for (int i = 0; i <= count.value; i++) {
            instantiate();
        }
        currentOffsetX = currentOffsetZ;
        currentOffsetZ += 10;
        EndAction(true);
    }

    private void instantiate() {
        int xPos = UnityEngine.Random.Range(0, 10) + currentOffsetX;
        int zPos = UnityEngine.Random.Range(0, 10) + currentOffsetZ;
        Vector3 position = new Vector3(xPos, yPos, zPos);
        GameObject.Instantiate(enemy.value, position, Quaternion.identity);
    }

}

