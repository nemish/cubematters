using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleChecker : MonoBehaviour {

    private Vector3 offset;

    private int obstacleLayerMask = 1 << 8;
    private GameObject invisibleObstacle;
    private List<GameObject> pathFillers = new List<GameObject>();

    void Start() {
        invisibleObstacle = Resources.Load("prefabs/InvisibleObstacle", typeof(GameObject)) as GameObject;
    }

    public float GetObstacleOffsetStep(MoveDirection dir, int step) {
        int toObstacleStep = step;
        float checkDistance = step + 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir.AsVector(), out hit, checkDistance, obstacleLayerMask)) {
            toObstacleStep = Mathf.RoundToInt(Vector3.Distance(transform.parent.position, hit.transform.position + getObstacleCompensationVector(dir)));
        }
        return toObstacleStep;
    }

    public bool WillCollide(MoveDirection dir, int step) {
        foreach (GameObject pathFiller in pathFillers) {
            Destroy(pathFiller);
        }

        if (Physics.Raycast(transform.position, dir.AsVector(), step, obstacleLayerMask)) {
            return true;
        }
        return false;
    }

    private Vector3 getObstacleCompensationVector(MoveDirection dir) {
        if (dir.IsLeft()) {
            return new Vector3(1.5f, -0.5f, -0.5f);
        } else if (dir.IsRight()) {
            return new Vector3(-0.5f, -0.5f, -0.5f);
        } else if (dir.IsUp()) {
            return new Vector3(0.5f, -0.5f, -1.5f);
        } else {
            return new Vector3(0.5f, -0.5f, 0.5f);
        }
    }

    public void FillPathVolume(MoveDirection dir, int step) {
        int index = 0;
        while (index <= step) {
            pathFillers.Add((GameObject)Instantiate(invisibleObstacle, transform.position + dir.AsVector() * index, transform.rotation));
            ++index;
        }
    }

}
