using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NodeCanvas.Framework{

    /// <summary>
    /// Global Blackboards are accessible from any BBParameter. Their name must be unique
    /// </summary>
    [ExecuteInEditMode]
	public class GlobalBlackboard : Blackboard {

		///A list of all the current active global blackboards
		public static List<GlobalBlackboard> allGlobals = new List<GlobalBlackboard>();

		new public string name{
			get {return base.name;}
			set
			{
				if (base.name != value){
					base.name = value;
					CheckUniqueName();
				}
			}
		}
		


		///A convenient way to find and get a global blackboard by it's name
		public static GlobalBlackboard Find(string name){
			if (!Application.isPlaying)
				return FindObjectsOfType<GlobalBlackboard>().Where(b => b.name == name).FirstOrDefault();
			return allGlobals.Find(b => b.name == name);
		}

		void OnAwake(){
			if (enabled && !allGlobals.Contains(this)){
				allGlobals.Add(this);
			}
		}

		void OnEnable(){
			if (!allGlobals.Contains(this)){
				allGlobals.Add(this);
			}
			CheckUniqueName();
		}

		void OnDisable(){
			allGlobals.Remove(this);
		}

		bool CheckUniqueName(){
			if (allGlobals.Find(b => b.name == this.name && b != this)){
				Debug.LogError(string.Format("There is a duplicate <b>GlobalBlackboard</b> named '{0}' in the scene. Please rename it", name), this);
				if (Application.isPlaying){
					DestroyImmediate(this.gameObject);
				}
				return false;
			}

			if (Application.isPlaying){
				DontDestroyOnLoad(this.gameObject);
			}
			return true;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		[UnityEditor.MenuItem("Window/NodeCanvas/Create/Scene Global Blackboard")]
		public static void CreateGlobalBlackboard(){
			var bb = new GameObject("@GlobalBlackboard").AddComponent<GlobalBlackboard>();
			bb.name = "Global";
			UnityEditor.Selection.activeObject = bb;
		}
			
		#endif
		
	}
}