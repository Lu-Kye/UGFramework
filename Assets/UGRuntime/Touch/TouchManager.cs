using System.Collections.Generic;
using UnityEngine;
using UGFramework.Editor.Inspector;

namespace UGFramework.Touch
{
    public class TouchManager : MonoBehaviour
    {
        public static TouchManager Instance { get; private set; }

        [ShowInInspector(IsReadonly = true)]
        TouchInfo[] _cachedTouches; 
        List<TouchInfo> _curTouches = new List<TouchInfo>();
        public List<TouchInfo> Touches { get { return _curTouches; } }

        bool _mobileCheckLostTouches = false;

        // Max touch count to check 
        [ShowInInspector]
        [SerializeField]
        uint _maxTouchCount = 2;
        public uint MaxTouchCount 
        {
            get { return _maxTouchCount; }
            set { _maxTouchCount = value; }
        }

        AbstractChecker[] _checkers;

        private const float INCHES_2_CENTIMETERS = 2.54f;
        public float ScreenPixelsPerCm
        {
            get
            {
                float fallbackDpi = 72f;
                #if UNITY_ANDROID
                    // Android MDPI setting fallback
                    // http://developer.android.com/guide/practices/screens_support.html
                    fallbackDpi = 160f;
                #elif (UNITY_WP8 || UNITY_WP8_1 || UNITY_WSA || UNITY_WSA_8_0)
                    // Windows phone is harder to track down
                    // http://www.windowscentral.com/higher-resolution-support-windows-phone-7-dpi-262
                    fallbackDpi = 92f;
                #elif UNITY_IOS
                    // iPhone 4-6 range
                    fallbackDpi = 326f;
                #endif

                return Screen.dpi == 0f ? fallbackDpi / INCHES_2_CENTIMETERS : Screen.dpi / INCHES_2_CENTIMETERS;
            }
        }

        void Awake()
        {
            Instance = this;
        }

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
