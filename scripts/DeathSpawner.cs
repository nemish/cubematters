using UnityEngine;
using System.Collections;

public class DeathSpawner : MonoBehaviour {

    public Transform part;

    private GameObject container1;
    private GameObject container2;

    private void Start() {
        container1 = createContainer();
        Transform line = createSequence(part, Vector3.left);
        Transform quad = createSequence(line, Vector3.forward);
        Transform cube = createSequence(quad, Vector3.up);
        cube.parent = container1.transform;
        container2 = Instantiate(container1, getPosition(), Quaternion.identity) as GameObject;
    }

    public void HandleDeath(Transform player1, Transform player2) {
        container1.transform.position = player1.position;
        container1.transform.rotation = player1.rotation;
        container2.transform.position = player2.position;
        container2.transform.rotation = player2.rotation;
        SetActiveRecursively(player1.gameObject, false);
        SetActiveRecursively(player2.gameObject, false);
        SetActiveRecursively(container1, true);
        SetActiveRecursively(container2, true);
    }

    private Transform createSequence(Transform pr, Vector3 dir) {
        int i = 0;
        Vector3 pos;
        GameObject dynamicContainer = createContainer();
        Vector3 offset = new Vector3(-0.03f, 0.03f, 0.03f);
        while (i < 5) {
            pos = dynamicContainer.transform.position + dir * (float)i / 5f;
            Transform instantiatedPart = Instantiate(pr, pos, Quaternion.identity) as Transform;
            instantiatedPart.localPosition += offset;
            instantiatedPart.parent = dynamicContainer.transform;
            ++i;
        }
        return dynamicContainer.transform;
    }

    public static void SetActiveRecursively(GameObject rootObject, bool active) {
        rootObject.SetActive(active);

        foreach (Transform childTransform in rootObject.transform)
        {
            SetActiveRecursively(childTransform.gameObject, active);
        }
    }

    private GameObject createContainer() {
        GameObject cont = new GameObject("Container");
        cont.transform.position = getPosition();
        cont.SetActive(false);
        return cont;
    }

    private Vector3 getPosition() {
        return new Vector3(-5, 5, 3);
    }

}
