using System.Collections;
using UnityEngine;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Common actions")]
public class KillPlayer : ActionTask {

    public BBParameter<GameObject> player;
    private GameObject copy;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected override string OnInit(){
        initialPosition = player.value.transform.position;
        initialRotation = player.value.transform.rotation;
        // copy = Instantiate(player.value, initialPosition, initialRotation);
        // copy.SetActive(false);
        return null;
    }

    protected override void OnExecute(){
        PlayerAPI playerApi = player.value.GetComponent<PlayerAPI>();
        playerApi.Dead();
        StartCoroutine(RestartLevel());
        EndAction(true);
    }

    IEnumerator RestartLevel() {
        Debug.Log("Restaring level...");
        yield return new WaitForSeconds(1);
        // player.value.transform.position = initialPosition;
        // player.value.transform.rotation = initialRotation;
        // player.value.SetActive(true);
        // GameObject.Instantiate(player.value, initialPosition, initialRotation);
        Application.LoadLevel(Application.loadedLevelName);
    }
}