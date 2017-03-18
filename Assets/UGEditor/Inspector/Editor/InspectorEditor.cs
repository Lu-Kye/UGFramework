using UnityEngine;
using UGFramework.Editor.Inspector;

namespace UGFramework.Editor
{
    public class InspectorEditor<T> : UnityEditor.Editor
        where T : MonoBehaviour
    { 
        public T Target { get { return this.serializedObject.targetObject as T; } }

        public override void OnInspectorGUI()
        {
            InspectorUtility.DrawObject(this.Target);
        }
    }
}