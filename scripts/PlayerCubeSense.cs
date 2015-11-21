using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;

public class PlayerCubeSense : MonoBehaviour {

    void OnCollisionEnter(Collision collision){
        if (collision.gameObject.tag == Constants.enemyTag) {
            Graph.SendGlobalEvent("EnemyTouched");
        }
    }

}
