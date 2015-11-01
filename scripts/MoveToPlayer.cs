using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using DG.Tweening;

[Category("MainCamera actions")]
public class MoveToPlayer : ActionTask {

    public BBParameter<Transform> player;
    public BBParameter<float> moveTime = 2f;
    public BBParameter<Vector3> originalOffset;

    private Vector3 startMovePos;
    private float currentLerpTime = 0f;

    protected override string OnInit() {
        if (player.value == null || originalOffset.value == null) {
            return "Player or offset are not specified";
        }
        return null;
    }

    protected override void OnExecute() {
        startMovePos = agent.transform.position;
    }

    protected override void OnUpdate(){
        currentLerpTime += Time.deltaTime;

        if (currentLerpTime > moveTime.value) {
            currentLerpTime = moveTime.value;
        }
        float perc = currentLerpTime / moveTime.value;
        agent.transform.position = Vector3.Lerp(startMovePos, player.value.position, perc);

        if (Vector3.Distance(agent.transform.position - player.value.position, originalOffset.value) > 0.5f) {
            EndAction(true);
        }

        // Ease easeType = Ease.InOutQuart;
        // agent.transform.DOMove(player.value.position + originalOffset.value, moveTime.value).SetEase(easeType).OnComplete(
        //     () => EndAction(true)
        // );
    }
}
