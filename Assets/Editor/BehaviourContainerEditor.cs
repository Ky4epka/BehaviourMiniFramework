using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
namespace Main.Objects
{
    [CustomEditor(typeof(BehaviourContainer), true)]
    //[CanEditMultipleObjects]
    public class BehaviourContainer2Editor: Editor
    {
        private BehaviourContainer iContainer;
        private static Dictionary<BehaviourContainer, Dictionary<string, bool>> iGroupFoldState = new Dictionary<BehaviourContainer, Dictionary<string, bool>>();

        private void OnEnable()
        {
            iContainer = (BehaviourContainer)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
        
    }

}