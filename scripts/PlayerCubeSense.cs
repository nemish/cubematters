using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;

public class PlayerCubeSense : MonoBehaviour {

    void OnTriggerEnter(Collider other){
        if (System.Array.IndexOf(Constants.PlayerDeathTags, other.gameObject.tag) != -1) {
            Debug.Log("WE ARE GOING TO SEND EVENT!!!");
            // Graph.SendGlobalEvent(Constants.PlayerDeadEvent);
            Graph.SendGlobalEvent("PlayerDeadEvent");
        }
    }
}
