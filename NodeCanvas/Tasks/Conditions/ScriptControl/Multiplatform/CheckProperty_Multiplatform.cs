﻿using System.Reflection;
using NodeCanvas.Framework;
using NodeCanvas.Framework.Internal;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Serialization;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	[Name("Check Property (mp)")]
	[Category("✫ Script Control/Multiplatform")]
	[Description("Check a property on a script and return if it's equal or not to the check value")]
	public class CheckProperty_Multiplatform : ConditionTask {

		[SerializeField]
		private SerializedMethodInfo method;
		[SerializeField]
		private BBObjectParameter checkValue;
		[SerializeField]
		private CompareMethod comparison;

		private MethodInfo targetMethod{
			get {return method != null? method.Get() : null;}
		}

		public override System.Type agentType{
			get {return targetMethod != null? targetMethod.RTReflectedType() : typeof(Transform);}
		}

		protected override string info{
			get
			{
				if (method == null)
					return "No Property Selected";
				if (targetMethod == null)
					return string.Format("<color=#ff6457>* {0} *</color>", method.GetMethodString() );
				return string.Format("{0}.{1}{2}", agentInfo, targetMethod.Name, OperationTools.GetCompareString(comparison) + checkValue.ToString());
			}
		}

		//store the method info on agent set for performance
		protected override string OnInit(){
			if (targetMethod == null)
				return "CheckProperty Error";
			return null;
		}

		//do it by invoking method
		protected override bool OnCheck(){
			if (checkValue.varType == typeof(float))
				return OperationTools.Compare( (float)targetMethod.Invoke(agent, null), (float)checkValue.value, comparison, 0.05f );
			if (checkValue.varType == typeof(int))
				return OperationTools.Compare( (int)targetMethod.Invoke(agent, null), (int)checkValue.value, comparison);
			return object.Equals( targetMethod.Invoke(agent, null), checkValue.value );			
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnTaskInspectorGUI(){

			if (!Application.isPlaying && GUILayout.Button("Select Property")){
				System.Action<MethodInfo> MethodSelected = (method)=> {
					this.method = new SerializedMethodInfo(method);
					this.checkValue.SetType(method.ReturnType);
					comparison = CompareMethod.EqualTo;
				};

				if (agent != null){
					EditorUtils.ShowGameObjectMethodSelectionMenu(agent.gameObject, typeof(object), typeof(object), MethodSelected, 0, true, true);
				} else {
					var menu = new UnityEditor.GenericMenu();
					foreach (var t in UserTypePrefs.GetPreferedTypesList(typeof(Component), true))
						menu = EditorUtils.GetMethodSelectionMenu(t, typeof(object), typeof(object), MethodSelected, 0, true, true, menu);
					menu.ShowAsContext();
					Event.current.Use();
				}				
			}			

			if (targetMethod != null){
				GUILayout.BeginVertical("box");
				UnityEditor.EditorGUILayout.LabelField("Type", agentType.FriendlyName());
				UnityEditor.EditorGUILayout.LabelField("Property", targetMethod.Name);
				GUILayout.EndVertical();

				GUI.enabled = checkValue.varType == typeof(float) || checkValue.varType == typeof(int);
				comparison = (CompareMethod)UnityEditor.EditorGUILayout.EnumPopup("Comparison", comparison);
				GUI.enabled = true;
				EditorUtils.BBParameterField("Value", checkValue);
			}
		}

		#endif
	}
}