using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGFramework.Touch
{
    public class PinchChecker : AbstractChecker
    {
        public static PinchChecker Instance { get { return GetInstance<PinchChecker>(); } }

        public Action<PinchChecker> Handler = delegate {};

        bool _isPinching = false;
        public bool IsPinching 
        { 
            get { return _isPinching; }  
        }

        public Vector3 Mouse0Position { get; private set; } 
        public Vector3 PrevMouse0Position { get; private set; } 
        public Vector3 Mouse1Position { get; private set; } 
        public Vector3 PrevMouse1Position { get; private set; } 
        public float Delta { get; private set; }

        protected override void OnTouchesBegan(List<TouchInfo> touchInfos)
        {
            var touchedOnUI = true;
            for (int i = 0; i < touchInfos.Count; ++i)
            {
                touchedOnUI &= touchInfos[i].IsTouchedOnUI;
            }

            if (touchInfos.Count != 2 || touchedOnUI)
            {
                _state = CheckerState.Failed;
                return;
            }
            this.AddTrackingTouches(touchInfos);
            _state = CheckerState.SuccessAndChecking;
        }

        protected override void OnTouchesMoved(List<TouchInfo> trackingTouchInfos, List<TouchInfo> touchInfos)
        {
            if (trackingTouchInfos.Count != 2)
            {
                if (_isPinching)
                {
                    _isPinching = false;
                    this.Handler(this);
                }
                _state = CheckerState.Checking;
                return;
            }
            if (_state == CheckerState.SuccessAndChecking)
            {
                _isPinching = true;
                this.PrevMouse0Position = trackingTouchInfos[0].PrevPosition;
                this.Mouse0Position = trackingTouchInfos[0].Position;
                this.PrevMouse1Position = trackingTouchInfos[1].PrevPosition;
                this.Mouse1Position = trackingTouchInfos[1].Position;
                this.Delta = Vector3.Distance(this.Mouse0Position, this.Mouse1Position) - Vector3.Distance(this.PrevMouse0Position, this.PrevMouse1Position);
                this.Delta *= Time.deltaTime;
                this.Handler(this);
            }
        }

        protected override void OnTouchesEnd(List<TouchInfo> trackingTouchInfos, List<TouchInfo> touchInfos)
        {
            base.OnTouchesEnd(trackingTouchInfos, touchInfos);
            if (_isPinching)
            {
                _isPinching = false;
                this.Handler(this);
            }
        }
    }
}