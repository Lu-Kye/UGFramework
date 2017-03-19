using UnityEngine;
using UnityEditor;

namespace UGFramework.Editor.Inspector
{
    public class InspectorEditor<T> : UnityEditor.Editor
        where T : MonoBehaviour
    { 
        public T Target { get { return this.target as T; } }

        public override void OnInspectorGUI()
        {
            InspectorUtility.Setup(this.Target);
            Undo.RecordObject(this.Target, Target.GetType().Name);
            InspectorUtility.DrawObject(this.Target);
        }
    }
}