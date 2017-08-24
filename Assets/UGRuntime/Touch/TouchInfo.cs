using System;
using UnityEngine;
using UGFramework.UGEditor.Inspector;
using UnityEngine.EventSystems;

namespace UGFramework.Touch
{
    [Serializable]
    public class TouchInfo
    {
        [ShowInInspector]
        public int TouchID { get; private set; }

        [ShowInInspector]
        public TouchPhase Phase { get; set; } 

        [ShowInInspector] 
        // Mouse position
        public Vector3 Position { get; private set; }

        [ShowInInspector] 
        public Vector3 PrevPosition { get; private set; }

        [ShowInInspector]
        public bool IsTouchedOnUI { get; private set; }

    #if UNITY_EDITOR
        public TouchInfo InitByMouse()
        {
            this.TouchID = -1;
            this.PrevPosition = this.Position;
            this.Position = Input.mousePosition;
            this.Phase = TouchPhase.Moved;
            if (Input.GetMouseButtonDown(0) && Input.GetMouseButtonUp(0))
                this.Phase = TouchPhase.Canceled;
            else if (Input.GetMouseButtonUp(0))
                this.Phase = TouchPhase.Ended;
            else if(Input.GetMouseButtonDown(0))
                this.Phase = TouchPhase.Began;
            this.IsTouchedOnUI = EventSystem.current.IsPointerOverGameObject(this.TouchID);
            return this;
        }
    #endif

        public TouchInfo InitByTouch(UnityEngine.Touch touch)
        {
            this.TouchID = touch.fingerId;
            this.PrevPosition = touch.position - touch.deltaPosition;
            this.Position = touch.position;
            this.Phase = touch.phase;
            this.IsTouchedOnUI = EventSystem.current.IsPointerOverGameObject(this.TouchID);
            return this;
        }
    }
}
