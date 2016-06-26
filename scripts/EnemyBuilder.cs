using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBilder : MonoBehaviour {

    private Transform enemyCube;
    private Vector3 currentPosition;
    private int offset = 0;

    void Start() {
        enemyCube = transform.Find("EnemyCube");
        build();
    }

    private void build() {
        currentPosition = enemyCube.position;
        int cubesCount = UnityEngine.Random.Range(0, 4);
        for (int i = 0; i <= cubesCount; i++) {
            // Instantiate(enemyCube, currentPosition + new Vector3(currentPosition, Quaternion rotation);
        }
    }
}
