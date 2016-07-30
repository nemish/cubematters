using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAPI : MonoBehaviour {

    public GameObject explosion;
    List<Transform> childCubes = new List<Transform>();

    private void Start() {
        foreach (Transform child in transform) {
            if (child.tag == Constants.playerTag) {
                childCubes.Add(child);
            }
        }
    }

    public void Dead() {
        Debug.Log("Oh Fuck! I'm dead MFC!!!");
        gameObject.SetActive(false);
        foreach (Transform child in childCubes) {
            Instantiate(explosion, child.position + new Vector3(-0.5f, -0.5f, -0.5f), Random.rotation);
        }
        // Destroy(gameObject);
    }
}