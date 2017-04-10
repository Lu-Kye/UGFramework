using UnityEngine;
using UnityEditor;

namespace UGFramework.Editor.Inspector
{
    public class InspectorEditor<T> : UnityEditor.Editor
        where T : MonoBehaviour
    { 
        public T Target { get { return this.target as T; } }

        protected bool _inited = false;

        public override void OnInspectorGUI()
        {
            if (_inited == false)
                this.OnBeforeInit();
            
            this.OnDraw();

            if (_inited == false)
            {
                _inited = true;
                this.OnAfterInit();
            }
        }

        protected virtual void OnBeforeInit()
        {
            InspectorUtility.ForceFoldout = true;
        }

        protected virtual void OnDraw()
        {
            Undo.RecordObject(this.Target, this.Target.GetType().Name);                

            InspectorUtility.Setup(this.Target);
            InspectorUtility.DrawObject(this.Target);

            this.Repaint();
        }

        protected virtual void OnAfterInit()
        {
            InspectorUtility.ForceFoldout = false;
        }
    }
}