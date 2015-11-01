
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


namespace ParadoxNotion.Design{

	///A generic popup editor for all reference types
	public class GenericInspectorWindow : EditorWindow{

		public static GenericInspectorWindow current;
		public string inspectedID;
		public object value;

		private System.Type targetType;
		private Object context;
		private Vector2 scrollPos;

		void OnEnable(){
	        #if UNITY_5_2
	        titleContent = new GUIContent("Object Editor");
	        #else
	        title = "Object Editor";
	        #endif
			GUI.skin.label.richText = true;
			current = this;
		}

		void OnDisable(){
			current = null;
		}

		void OnGUI(){

			if (EditorApplication.isCompiling || targetType == null){
				Close();
				return;
			}

			//Begin undo check
			UndoManager.CheckUndo(context, "Blackboard External Inspector");

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("<size=14><b>{0}</b></size>", targetType.FriendlyName()) );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			value = EditorUtils.GenericField(targetType.FriendlyName(), value, targetType, null);
			GUILayout.EndScrollView();
			Repaint();

			//Check dirty
			UndoManager.CheckDirty(context);
		}

		public static void Show(string inspectedID, object o, System.Type t, Object context){

			var window = current == null? CreateInstance<GenericInspectorWindow>() : current;
			window.inspectedID = inspectedID;
			window.value = o;
			window.targetType = t;
			window.context = context;
			window.ShowUtility();
		}
	}
}

#endif