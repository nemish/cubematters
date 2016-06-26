using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;

public class PlayerCubeSense : MonoBehaviour {

    void OnTriggerEnter(Collider other){
        if (other.gameObject.tag == Constants.enemyTag) {
            Graph.SendGlobalEvent("EnemyTouched");
        }
    }

}
