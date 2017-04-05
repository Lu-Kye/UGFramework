using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UGFramework.Components
{
    public class WWWSyncAgent : MonoBehaviour
    {
        WWW _www;

        float _startTime;

        [Tooltip("Seconds")]
        public float Timeout = 10;

        IEnumerator WWWLoadAsset(string url)
        {
            _www = new WWW(url);
            yield return _www;
        }

        public WWW LoadAsset(string url)
        {
            var startTime = Time.realtimeSinceStartup;
            var iter = this.WWWLoadAsset(url);
            while (true)
            {
                var time = Time.realtimeSinceStartup;
                if (time - startTime > this.Timeout)
                    break;

                iter.MoveNext();
                if (string.IsNullOrEmpty(_www.error) == false)
                    return null;

                if (_www.isDone)
                    return _www;
            }
            return null;
        }
    }
}