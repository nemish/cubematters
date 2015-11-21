using UnityEngine;
using System.Collections;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

[Category("Scene actions")]
public class RestartFromCheckpoint : ActionTask {

    protected override void OnExecute() {
        Application.LoadLevel(Application.loadedLevel);
        EndAction(true);
    }

}
