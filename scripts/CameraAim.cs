using UnityEngine;
using System.Collections;

public class CameraAim : MonoBehaviour {

    public Transform player1;
    public Transform player2;
    public Camera mainCamera;

    private float initialDistanse;
    private float initialCamY;
    private Vector3 initialCamOffset;
    private Vector3 pos;

    void Start () {
        Init();
    }

    private void Init() {
        initialCamY = mainCamera.transform.position.y;
        if (isSingleCube()) {
            pos = player1.position;
            Vector3 relatedCamPos = new Vector3(pos.x, initialCamY, pos.z);
            initialCamOffset = relatedCamPos - pos;
        } else {
            pos = getMiddlePoint();
            initialDistanse = getPlayersDistance();
            initialCamOffset = mainCamera.transform.position - pos;
        }
        transform.position = pos;
    }

    public Vector3 getCamPosition() {
        Vector3 pos = transform.position + initialCamOffset;
        float height = getCameraAbsoluteHeight();
        if (!isSingleCube()) {
            height = getCamHeight();
        }
        return new Vector3(pos.x, Mathf.Max(height, initialCamY), pos.z);
    }

    private float getCameraAbsoluteHeight() {
        return (transform.position + initialCamOffset).y;
    }

    private Vector3 getMiddlePoint() {
        return player2.position - (player2.position - player1.position) / 2;
    }

    private float getPlayersDistance() {
        return Vector3.Distance(player2.position, player1.position);
    }

    public float getCamHeight() {
        float playersHeightsDifference = Mathf.Abs(player1.position.y - player2.position.y);
        return getCameraAbsoluteHeight() * getPlayersDistance() / initialDistanse + playersHeightsDifference;
    }

    private bool isSingleCube() {
        return player2 == null;
    }

    private void Update() {
        if (player1 == null) {
            return;
        }

        if (isSingleCube()) {
            transform.position = player1.position;
        } else {
            transform.position = getMiddlePoint();
        }
    }

}
