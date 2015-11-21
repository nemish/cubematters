using UnityEngine;
using NodeCanvas.Framework;
using DG.Tweening;
using ParadoxNotion.Design;

[Category("Player actions")]
public class MoveCube : ActionTask {

    public BBParameter<string> direction;
    public BBParameter<float> moveTime = 0.5f;

    private Vector3 tPos;
    private Vector3 tRot;

    protected override void OnExecute(){
        Ease easeType = Ease.InOutQuart;
        Vector3 oPos = agent.transform.position;
        Quaternion oRot = agent.transform.rotation;

        if (direction.value == "left") {

            tPos = new Vector3(oPos.x - 1, oPos.y, oPos.z);
            tRot = new Vector3(0f, 0f, 0f);

            agent.transform.position = tPos;
            agent.transform.rotation = Quaternion.Euler(0f, 0f, -90f);

        } else if (direction.value == "right") {

            tPos = new Vector3(oPos.x + 1, oPos.y, oPos.z);
            tRot = new Vector3(0f, 0f, -90f);

        } else if (direction.value == "forward") {

            tPos = new Vector3(oPos.x, oPos.y, oPos.z + 1);
            tRot = new Vector3(0f, 0f, 0f);

            agent.transform.position = tPos;
            agent.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        } else {

            tPos = new Vector3(oPos.x, oPos.y, oPos.z - 1);
            tRot = new Vector3(-90f, 0f, 0f);

        }
        agent.transform.DORotate(tRot, moveTime.value).SetEase(easeType).OnComplete(
            () => ResetTransform(tPos, oRot)
        );
    }

    private void ResetTransform(Vector3 pos, Quaternion rot) {
        agent.transform.rotation = rot;
        agent.transform.position = pos;
        EndAction(true);
    }
}
