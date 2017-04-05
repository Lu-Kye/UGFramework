using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UGFramework.Utility;

namespace UGFramework.Log
{
    public class LogWriter : MonoBehaviour
    {
        static LogWriter _instance = null;
        public static LogWriter Instance 
        {
            get
            {
                if (_instance != null)
                    return _instance;
                    
                var go = new GameObject("LogWriter");
                _instance = go.AddComponent<LogWriter>();
                GameObject.DontDestroyOnLoad(go);
                return _instance;
            }
        }

        LinkedList<string> _msgs = new LinkedList<string>();

        string LogFilePath
        {
            get
            {
                return PathUtility.PersistentPath + "/log/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            }
        }

        public void Write(string msg)
        {
            lock (_msgs)
            {
                _msgs.AddLast(msg);
            }
        }

        void Update()
        {
            lock (_msgs)
            {
                if (_msgs.Count <= 0)
                return;

                var sb = new StringBuilder();
                var iter = _msgs.GetEnumerator();
                while (iter.MoveNext())
                {
                    sb.AppendLine(iter.Current);
                }
                FileUtility.WriteFile(this.LogFilePath, sb.ToString(), true);

                _msgs.Clear();
            }
        }

        void OnApplicationQuit()
        {
            GameObject.Destroy(this.gameObject);        
        }
    }
}