using UnityEngine;

namespace UGFramework.Components
{
    [ExecuteInEditMode]
    public class Follower : MonoBehaviour 
    {
        public Transform Target;

        // Offset position relative to target
        public Vector3 Offset = Vector3.zero;

        protected virtual void Awake()
        {
        }

        void Update()
        {
            if (this.Target == null)
                return;

            var target = Target;
            var source = this.transform;
            this.UpdatePosition(source, target);
        }

        protected virtual void UpdatePosition(Transform source, Transform target)
        {
            source.position = target.position + this.Offset;
        }
    }
}
