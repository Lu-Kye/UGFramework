using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGFramework.Touch
{
    public class TouchChecker : AbstractChecker
    {
        public static TouchChecker Instance { get { return GetInstance<TouchChecker>(); } }

        public Action<TouchChecker> Handler = delegate {};

        public Vector3 TouchPosition;

        [SerializeField]
        float _minMoveDis;
        float _moveDis;

        protected override void OnTouchesBegan(List<TouchInfo> touchInfos)
        {
            TouchInfo touchInfo = null;
            for (int i = 0; i < touchInfos.Count; ++i)
            {
                if (touchInfos[i].Phase != TouchPhase.Began || touchInfos[i].IsTouchedOnUI)
                    continue;
                touchInfo = touchInfos[i]; 
                break;
            }
            if (touchInfo == null)
                return;

            _state = CheckerState.Began;
            this.AddTrackingTouch(touchInfo);
            this.TouchPosition = touchInfos[0].Position;
            _moveDis = 0;
        }

        protected override void OnTouchesMoved(List<TouchInfo> trackingTouchInfos, List<TouchInfo> touchInfos)
        {
            base.OnTouchesMoved(trackingTouchInfos, touchInfos);
            _moveDis = Vector3.Distance(trackingTouchInfos[0].Position, this.TouchPosition);
        }

        protected override void OnTouchesEnd(List<TouchInfo> trackingTouchInfos, List<TouchInfo> touchInfos)
        {
            base.OnTouchesEnd(trackingTouchInfos, touchInfos);
            if (_moveDis < _minMoveDis)
            {
                _state = CheckerState.Success;
                this.Handler(this);
            }
        }
    }
}