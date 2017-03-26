using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGFramework.Touch
{
    public class DragChecker : AbstractChecker
    {
        public static DragChecker Instance { get { return GetInstance<DragChecker>(); } }

        public Action<DragChecker> Handler = delegate {};

        [SerializeField]
        float _delay;

        bool _isDragging = false;
        public bool IsDragging 
        { 
            get { return _isDragging; }  
        }

        public Vector3 PrevMousePosition;
        public Vector3 MousePosition;

        UnityEngine.Coroutine _tryStartDraggingCo;
        IEnumerator TryStartDragging(TouchInfo touchInfo)
        {
            yield return new WaitForSeconds(_delay);
            if (touchInfo.Phase == TouchPhase.Ended)
            {
                _state = CheckerState.Failed;
                yield break;
            }
            _state = CheckerState.SuccessAndChecking;
            this.AddTrackingTouch(touchInfo);
            this.PrevMousePosition = touchInfo.Position;
            this.MousePosition = Input.mousePosition; 
        }

        void StartTryDragging(TouchInfo touchInfo)
        {
            this.StopTryDragging();
            _tryStartDraggingCo = this.StartCoroutine(this.TryStartDragging(touchInfo));
        }

        void StopTryDragging()
        {
            if (_tryStartDraggingCo != null) this.StopCoroutine(_tryStartDraggingCo);
        }

        protected override void OnTouchesBegan(List<TouchInfo> touchInfos)
        {
            if (touchInfos.Count > 1)
            {
                if (_state == CheckerState.SuccessAndChecking)
                {
                    _isDragging = false;
                    this.Handler(this);
                }
                _state = CheckerState.Failed;
                return;
            }
            base.OnTouchesBegan(touchInfos);
            this.StartTryDragging(touchInfos[0]);
        }

        protected override void OnTouchesMoved(List<TouchInfo> trackingTouchInfos, List<TouchInfo> touchInfos)
        {
            if (touchInfos.Count > 1)
            {
                this.StopTryDragging();
                _state = CheckerState.Failed;
                return;
            }
            if (_state == CheckerState.SuccessAndChecking)
            {
                _isDragging = true;
                this.MousePosition = trackingTouchInfos[0].Position;
                this.Handler(this);
                this.PrevMousePosition = this.MousePosition;
            }
        }

        protected override void OnTouchesEnd(List<TouchInfo> trackingTouchInfos, List<TouchInfo> touchInfos)
        {
            this.StopTryDragging();
            if (_state == CheckerState.SuccessAndChecking)
            {
                _isDragging = false;
                _state = CheckerState.Possible;
                this.Handler(this);
            }
            base.OnTouchesEnd(trackingTouchInfos, touchInfos);
        }
    }
}