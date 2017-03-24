using UnityEngine;

namespace UGFramework.Touch
{
    public class TouchInfo
    {
        public TouchPhase Phase { get; set; } 

        // Mouse position
        public Vector3 Position { get; private set; }

        public Vector3 PrevPosition { get; private set; }

    #if UNITY_EDITOR
        public TouchInfo InitByMouse()
        {
            this.PrevPosition = this.Position;
            this.Position = Input.mousePosition;
            this.Phase = TouchPhase.Moved;
            if (Input.GetMouseButtonDown(0) && Input.GetMouseButtonUp(0))
                this.Phase = TouchPhase.Canceled;
            else if (Input.GetMouseButtonUp(0))
                this.Phase = TouchPhase.Ended;
            else if(Input.GetMouseButtonDown(0))
                this.Phase = TouchPhase.Began;
            return this;
        }
    #endif

        public TouchInfo InitByTouch(Touch touch)
        {
            this.PrevPosition = touch.position - touch.deltaPosition;
            this.Position = touch.position;
            this.Phase = touch.phase;
            return this;
        }
    }
}