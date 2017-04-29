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

    	public float MinimumScaleDistanceToRecognize = 0.5f;
        float _firstDistance = 0;
        float _beganDistance = 0;
        float _previousDistance = 0;

        public Vector3 Mouse0Position { get; private set; } 
        public Vector3 PrevMouse0Position { get; private set; } 
        public Vector3 Mouse1Position { get; private set; } 
        public Vector3 PrevMouse1Position { get; private set; } 

        float _deltaScale;
        public float DeltaScale 
        {
            get { return _deltaScale; }
            private set
            {
                _deltaScale = value * 10;
            }
        }

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
            _firstDistance = this.DistanceBetweenTrackedTouches(touchInfos);
            _previousDistance = _firstDistance;
            _state = CheckerState.Began;
        }

        float DistanceBetweenTrackedTouches(List<TouchInfo> trackingTouchInfos)
        {
            this.PrevMouse0Position = trackingTouchInfos[0].PrevPosition;
            this.Mouse0Position = trackingTouchInfos[0].Position;
            this.PrevMouse1Position = trackingTouchInfos[1].PrevPosition;
            this.Mouse1Position = trackingTouchInfos[1].Position;

            var distance = Vector3.Distance(this.Mouse0Position, this.Mouse1Position);
            return Mathf.Max(0.0001f, distance) / TouchManager.Instance.ScreenPixelsPerCm;
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
                _state = CheckerState.Failed;
                return;
            }
            
            var currentDistance = this.DistanceBetweenTrackedTouches(trackingTouchInfos);
            if (_state == CheckerState.Began || _state == CheckerState.Checking)
            {
                // if (Mathf.Abs(currentDistance - _previousDistance) < this.MinimumScaleDistanceToRecognize)
                // {
                //     _state = CheckerState.Checking;
                //     return;
                // }

                _isPinching = true;
                this.DeltaScale = (currentDistance - _previousDistance) / _firstDistance;
                _beganDistance = currentDistance;
                _state = CheckerState.SuccessAndChecking;
            }
            else if (_state == CheckerState.SuccessAndChecking)
            {
                _isPinching = true;
                this.DeltaScale = (currentDistance - _previousDistance) / _beganDistance;
            }

            _previousDistance = currentDistance;
            this.Handler(this);
        }

        protected override void OnTouchesEnd(List<TouchInfo> trackingTouchInfos, List<TouchInfo> touchInfos)
        {
            base.OnTouchesEnd(trackingTouchInfos, touchInfos);
            if (_isPinching)
            {
                _isPinching = false;
                this.Handler(this);
                _state = CheckerState.Success;
            }
        }
    }
}