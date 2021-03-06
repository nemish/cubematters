using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.BehaviourTrees{

	[Name("FSM")]
	[Category("Nested")]
	[Description("NestedFSM can be assigned an entire FSM. This node will return Running for as long as the FSM is Running. If a Success or Failure State is selected, then it will return Success or Failure as soon as the Nested FSM enters that state at which point the FSM will also be stoped. If the Nested FSM ends otherwise, this node will return Success.")]
	[Icon("FSM")]
	public class NestedFSM : BTNode, IGraphAssignable{

		[SerializeField]
		protected BBParameter<FSM> _nestedFSM;

		private readonly Dictionary<FSM, FSM> instances = new Dictionary<FSM, FSM>();

		public string successState;
		public string failureState;

		public FSM nestedFSM{
			get {return _nestedFSM.value;}
			set {_nestedFSM.value = value;}
		}

		public Graph nestedGraph{
			get {return nestedFSM;}
			set {nestedFSM = (FSM)value;}
		}

		public override string name{
			get {return base.name.ToUpper();}
		}

		/////////

		protected override Status OnExecute(Component agent, IBlackboard blackboard){

			if (nestedFSM == null || nestedFSM.primeNode == null){
				return Status.Failure;
			}

			if (status == Status.Resting){
				CheckInstance();
			}

			if (status == Status.Resting || nestedFSM.isPaused){
				status = Status.Running;
				nestedFSM.StartGraph(agent, blackboard, OnFSMFinish);
			}

			if (!string.IsNullOrEmpty(successState) && nestedFSM.currentStateName == successState){
				nestedFSM.Stop();
				return Status.Success;
			}

			if (!string.IsNullOrEmpty(failureState) && nestedFSM.currentStateName == failureState){
				nestedFSM.Stop();
				return Status.Failure;
			}

			return status;
		}

		void OnFSMFinish(bool success){
			if (status == Status.Running){
				status = Status.Success;
			}
		}

		protected override void OnReset(){
			if (IsInstance(nestedFSM)){
				nestedFSM.Stop();
			}
		}

		public override void OnGraphPaused(){
			if (IsInstance(nestedFSM)){
				nestedFSM.Pause();
			}
		}

		public override void OnGraphStoped(){
			if (IsInstance(nestedFSM)){
				nestedFSM.Stop();
			}
		}

		bool IsInstance(FSM fsm){
			return instances.Values.Contains(fsm);
		}

		void CheckInstance(){

			if (IsInstance(nestedFSM)){
				return;
			}

			FSM instance = null;
			if (!instances.TryGetValue(nestedFSM, out instance)){
				instance = Graph.Clone<FSM>(nestedFSM);
				instances[nestedFSM] = instance;
			}

            instance.agent = graphAgent;
		    instance.blackboard = graphBlackboard;
			nestedFSM = instance;
		}

		////////////////////////////
		//////EDITOR AND GUI////////
		////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){

			GUILayout.Label(string.Format("SubFSM\n{0}", _nestedFSM) );
			if (nestedFSM == null){
				if (!Application.isPlaying && GUILayout.Button("CREATE NEW")){
					Node.CreateNested<FSM>(this);
				}
			}
		}

		protected override void OnNodeInspectorGUI(){

		    EditorUtils.BBParameterField("Nested FSM", _nestedFSM);

		    if (nestedFSM == null){
		    	return;
		    }

		    successState = EditorUtils.StringPopup("Success State", successState, nestedFSM.GetStateNames().ToList(), false, true);
		    failureState = EditorUtils.StringPopup("Failure State", failureState, nestedFSM.GetStateNames().ToList(), false, true);

	    	var defParams = nestedFSM.GetDefinedParameters();
	    	if (defParams.Length != 0){

		    	EditorUtils.TitledSeparator("Defined SubFSM Parameters");
		    	GUI.color = Color.yellow;
		    	UnityEditor.EditorGUILayout.LabelField("Name", "Type");
				GUI.color = Color.white;
		    	var added = new List<string>();

		    	foreach(var bbVar in defParams){
		    		if (!added.Contains(bbVar.name)){
			    		UnityEditor.EditorGUILayout.LabelField(bbVar.name, bbVar.varType.FriendlyName());
			    		added.Add(bbVar.name);
			    	}
		    	}

		    	if (GUILayout.Button("Check/Create Blackboard Variables")){
		    		nestedFSM.CreateDefinedParameterVariables(graphBlackboard);
		    	}
		    }
		}

		#endif
	}
}