using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace cubematters {

	[Category("MainCamera actions")]
	public class InitCamParams : ActionTask{

        public BBParameter<Transform> player;
        public BBParameter<Vector3> offset;

		protected override void OnExecute(){
            offset.value = agent.transform.position - player.value.position;
			EndAction(true);
		}
	}
}