using UnityEngine;

namespace UGFramework.Editor
{
    public class UGEditor<T> : UnityEditor.Editor
        where T : MonoBehaviour
    { 
        public T Target { get { return this.serializedObject as T; } }
    }
}