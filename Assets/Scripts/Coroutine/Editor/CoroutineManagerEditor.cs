using UnityEditor;
using UGFramework.Editor;

namespace UGFramework.Coroutine
{
    [CustomEditor(typeof(CoroutineManager))]
    public class CoroutineManagerEditor : UGEditor<CoroutineManager>
    {
        public override void OnInspectorGUI() 
        {
            // this.serializedObject.FindProperty("_coroutineGroups").objectReferenceValue;
        }
    }
}