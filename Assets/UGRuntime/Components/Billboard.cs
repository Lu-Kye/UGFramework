using UnityEngine;

namespace UGFramework.Components
{
    [ExecuteInEditMode]
    public class Billboard : Follower
    {
        public Canvas Canvas;
        public RectTransform RectTransform { get; private set; }

        protected override void Awake()
        {
            this.RectTransform = this.transform as RectTransform;
        }

        protected override void UpdatePosition(Transform source, Transform target)
        {
            if (this.Canvas == null)
                return;

            // Convert target position to screen position
            var screenPoint = Camera.main.WorldToScreenPoint(target.position + this.Offset);
            Vector2 localPoint;

            // Convert screen position to canvas local position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.Canvas.transform as RectTransform, screenPoint, this.Canvas.worldCamera, out localPoint);
            this.RectTransform.anchoredPosition = localPoint;
        }
    }
}
