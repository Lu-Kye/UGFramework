using System.Collections.Generic;
using UnityEngine;

namespace UGFramework.Touch
{
    public class TouchManager : MonoBehaviour
    {
        TouchInfo[] _cachedTouches; 
        List<TouchInfo> _curTouches = new List<TouchInfo>();
        public List<TouchInfo> Touches { get { return _curTouches; } }

        bool _mobileCheckLostTouches = false;

        // Max touch count to check 
        uint _maxTouchCount = 2;
        public uint MaxTouchCount 
        {
            get { return _maxTouchCount; }
            set { _maxTouchCount = value; }
        }

        AbstractChecker[] _checkers;

        void Start()
        {
            _checkers = new AbstractChecker[AbstractChecker.Instances.Count];
            var iter = AbstractChecker.Instances.Values.GetEnumerator();
            for (int i = 0; i < _checkers.Length; ++i)
            {
                iter.MoveNext();
                _checkers[i] = iter.Current;
            }
            this.Setup();
        }

        public void Setup()
        {
            _cachedTouches = new TouchInfo[_maxTouchCount];
            for (int i = 0; i < _maxTouchCount; ++i)
            {
                _cachedTouches[i] = new TouchInfo();
            }
        }

        void Update() 
        {
    #if UNITY_EDITOR
            // When using unity remote, dont check mouse position
            if (Input.touchCount == 0)
            {
                if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
                    _curTouches.Add(_cachedTouches[0].InitByMouse());
            }
    #endif

            if (Input.touchCount > 0)
            {
                _mobileCheckLostTouches = true;
                for (int i = 0; i < Mathf.Min(this.MaxTouchCount, Input.touches.Length); ++i)
                {
                    var touch = Input.touches[i];
                    _curTouches.Add(_cachedTouches[touch.fingerId].InitByTouch(touch));  
                }
            }
            else if (_mobileCheckLostTouches)
            {
                _mobileCheckLostTouches = false;
                for (int i = 0; i < _cachedTouches.Length; ++i)
                {
                    var touchInfo = _cachedTouches[i];
                    if (touchInfo.Phase == TouchPhase.Ended)
                        continue;
                    touchInfo.Phase = TouchPhase.Ended;
                    _curTouches.Add(touchInfo);
                }
            }

            if (_curTouches.Count == 0)
                return;

            // Debug.Log(_curTouches[0].Phase.ToString());
            // Debug.Log(_curTouches.Count);

            // Do checking
            for (int i = 0; i < _checkers.Length; ++i)
            {
                var checker = _checkers[i];
                if (checker.Enable == false)
                    continue;
                checker.Check(_curTouches);
            }
            _curTouches.Clear();
        }
    }
}
