using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class PlayerAPI : MonoBehaviour {

    public GameObject explosion;
    List<Transform> childCubes = new List<Transform>();

    public void InitAPI() {
        childCubes.Clear();
        foreach (Transform child in transform) {
            if (child.tag == Constants.PLAYER_CUBE_CONTAINER_TAG) {
                childCubes.Add(child);
            }
        }
    }

    public void Dead() {
        gameObject.SetActive(false);
        foreach (Transform child in childCubes) {
            Instantiate(explosion, child.position + new Vector3(-0.5f, -0.5f, -0.5f), Random.rotation);
        }
        // Destroy(gameObject);
    }

    public List<Transform> GetTouchingOtherPlayCubes() {
        List<Transform> touchingCubes = new List<Transform>();
        foreach (Transform child in childCubes) {
            CubeManager mgr = child.GetComponent<CubeManager>();
            foreach (Transform cube in mgr.GetPlayCubesInTouch()) {
                touchingCubes.Add(cube);
            }
        }
        return touchingCubes.Distinct().ToList();
    }

    public List<Transform> GetChildPlayCubes() {
        return childCubes;
    }
}