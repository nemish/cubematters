using System.Collections.Generic;
using System;
using UnityEngine;


namespace ParadoxNotion.Services {

    ///Singleton. Automatically added when needed, collectively calls methods that needs updating amongst other things relative to MonoBehaviours
    public class MonoManager : MonoBehaviour {

        public event Action onUpdate;
        public event Action onLateUpdate;
        public event Action onFixedUpdate;
        public event Action onGUI;


        private static bool isQuiting;
        private static MonoManager _current;
        public static MonoManager current {
            get
            {
                if ( _current == null && !isQuiting ) {
                    _current = FindObjectOfType<MonoManager>();
                    if ( _current == null )
                        _current = new GameObject("_MonoManager").AddComponent<MonoManager>();
                }
                return _current;
            }
        }

        ///Creates MonoManager singleton
        public static void Create() { _current = current; }

        public static void AddUpdateMethod(Action method) { current.onUpdate += method; }
        public static void RemoveUpdateMethod(Action method) { current.onUpdate -= method; }

        public static void AddLateUpdateMethod(Action method) { current.onLateUpdate += method; }
        public static void RemoveLateUpdateMethod(Action method) { current.onLateUpdate -= method; }

        public static void AddFixedUpdateMethod(Action method) { current.onFixedUpdate += method; }
        public static void RemoveFixedUpdateMethod(Action method) { current.onFixedUpdate -= method; }

        public static void AddOnGUIMethod(Action method) { current.onGUI += method ; }
        public static void RemoveOnGUIMethod(Action method) { current.onGUI -= method ; }



        void Awake() {
            if ( _current != null && _current != this ) {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            _current = this;
        }

        void OnApplicationQuit() { isQuiting = true; }

        void Update(){
            if (onUpdate != null){
                onUpdate();
            }
        }

        void LateUpdate(){
            if (onLateUpdate != null){
                onLateUpdate();
            }
        }

        void FixedUpdate(){
            if (onFixedUpdate != null){
                onFixedUpdate();
            }
        }

        void OnGUI(){
            if (onGUI != null){
                onGUI();
            }
        }
    }
}