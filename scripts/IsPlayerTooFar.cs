using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("MainCamera actions")]
public class IsPlayerTooFar : ConditionTask {

    public BBParameter<float> offsetX = 2f;
    public BBParameter<float> offsetY = 2f;
    public BBParameter<float> offsetZ = 2f;
    public BBParameter<Vector3> originalOffset;
    public BBParameter<Transform> player;

    protected override string OnInit() {
        if (player.value == null) {
            return "Player not specified";
        }
        if (originalOffset.value == null) {
            return "Offset not specified";
        }
        return null;
    }

    protected override bool OnCheck() {
        Vector3 currentOffset = agent.transform.position - player.value.position - originalOffset.value;
        Debug.Log(currentOffset);
        if ((Mathf.Abs(currentOffset.x) > offsetX.value) || (Mathf.Abs(currentOffset.y) > offsetY.value) || (Mathf.Abs(currentOffset.z) > offsetZ.value)) {
            return true;
        }
        return false;
    }
}
