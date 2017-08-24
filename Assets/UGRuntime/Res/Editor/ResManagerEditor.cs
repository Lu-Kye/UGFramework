using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UGFramework.UGEditor.Inspector;

namespace UGFramework.Res
{
    [CustomEditor(typeof(ResManager), true)]
    public class ResManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            InspectorUtility.Setup(this.serializedObject.targetObject);
            InspectorUtility.DrawObject(this.serializedObject.targetObject);
            this.Repaint();
        }
    }
}