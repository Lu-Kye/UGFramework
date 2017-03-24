using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGFramework.Touch
{
    public enum CheckerState
    {
        Possible, // Havnt begun checking
        Began, // Began checking
        Checking, // Still checking
        Failed, // Checked and failed
        Success, // Checked and succeed
        SuccessAndChecking, // Succeed but still checking
    }

    public abstract class AbstractChecker : MonoBehaviour
    {
        static Dictionary<Type, AbstractChecker> _instances = new Dictionary<Type, AbstractChecker>();
        public static Dictionary<Type, AbstractChecker> Instances { get { return _instances; } }
        protected virtual void Awake()
        {
            if (_instances.ContainsKey(this.GetType()))
                _instances.Remove(this.GetType());
            _instances.Add(this.GetType(), this); 
        }

        public static T GetInstance<T>()
            where T : AbstractChecker, new()
        {
            AbstractChecker instance;
            _instances.TryGetValue(typeof(T), out instance);
            if (instance != null)
                return instance as T;
            return null;
        }

        [SerializeField]
        protected CheckerState _state;
        public CheckerState State
        {
            get { return _state; }
        }

        bool _enable = true;
        public bool Enable
        {
            get { return _enable; }
            set { _enable = value; }
        }

        List<TouchInfo> _trackingTouches = new List<TouchInfo>();
        protected void AddTrackingTouch(TouchInfo touchInfo)
        {
            if (_trackingTouches.Contains(touchInfo))
                return;
            _trackingTouches.Add(touchInfo);
        }
        protected void AddTrackingTouches(List<TouchInfo> touchInfos)
        {
            for (int i = 0; i < touchInfos.Count; ++i)
            {
                var touchInfo = touchInfos[i];
                this.AddTrackingTouch(touchInfo);
            }
        }

        List<TouchInfo> _filteredTrackingTouches = new List<TouchInfo>();
        protected bool FilterByTrackine(List<TouchInfo> touchInfos)
        {
            _filteredTrackingTouches.Clear();
            for (int i = 0; i < touchInfos.Count; ++i) 
            {
                var touchInfo = touchInfos[i];
                if (_trackingTouches.Contains(touchInfo) == false)
                    continue;
                _filteredTrackingTouches.Add(touchInfo);
            }
            return _filteredTrackingTouches.Count > 0;
        }

        protected virtual void Init()
        {
            _state = CheckerState.Possible;
        }

        public virtual void Reset()
        {
            _state = CheckerState.Possible;
        }

        public virtual void Check(List<TouchInfo> touchInfos)
        {
            bool touchesBegan = false;
            bool touchesMoved = false;
            bool touchesEnded = false;

            for (int i = 0; i < touchInfos.Count; ++i) 
            {
                var touchInfo = touchInfos[i];
                switch (touchInfo.Phase)
                {
                    case TouchPhase.Began:
                        if (touchesBegan == false)
                        {
                            touchesBegan = true;
                            this.OnTouchesBegan(touchInfos);
                        }
                        break;
                    case TouchPhase.Moved:
                        if (touchesMoved == false && this.FilterByTrackine(touchInfos)) 
                        {
                            this.OnTouchesMoved(_filteredTrackingTouches, touchInfos);
                        }
                        break;
                    case TouchPhase.Ended:
                        if (touchesEnded == false && this.FilterByTrackine(touchInfos)) 
                        {
                            this.OnTouchesEnd(_filteredTrackingTouches, touchInfos);
                        }
                        break;
                }
            }
        }

        protected virtual void OnTouchesBegan(List<TouchInfo> touchInfos)
        {
            _state = CheckerState.Began;
        }

        protected virtual void OnTouchesMoved(List<TouchInfo> trackingTouchInfos, List<TouchInfo> touchInfos)
        {
            _state = CheckerState.Checking;
        }

        protected virtual void OnTouchesEnd(List<TouchInfo> trackingTouchInfos, List<TouchInfo> touchInfos)
        {
            _state = CheckerState.Possible;
            _trackingTouches.Clear();
        }
    }
}