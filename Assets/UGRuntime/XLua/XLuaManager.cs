#if XLUA

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace UGFramework
{
    public class XLuaManager : MonoBehaviour
    {
        public static XLuaManager Instance { get; private set; }
        XLuaManager() {}

        public static readonly LuaEnv LuaEnv = new LuaEnv();

        float _lastGCTime = 0;
        // Lua gc interval (seconds)
        public float GCInterval = 1.0f;

        public string[] PreloadLuas;

        Dictionary<string, byte[]> _loadedLuas = new Dictionary<string, byte[]>();

        Action _start;
        Action _update;
        Action _onDestroy;
        Action _test;

        bool _gameStarted = false;

        void Awake()
        {
            Instance = this;
            LuaEnv.AddLoader((ref string filepath) => {
                return this.LoadBytes(filepath);
            });
        }

        public void Init()
        {
            _gameStarted = false;
            _loadedLuas.Clear();
            for (int i = 0; i < this.PreloadLuas.Length; i++) 
            {
                this.Load(this.PreloadLuas[i]);
            }
            this.InitLuaFunctions();
        }

        public void LuaStart()
        {
            if (_gameStarted == false && _start != null)
            {
                _gameStarted = true;
                _start();
            }
        }

        void InitLuaFunctions()
        {
            LuaEnv.Global.Get("Start", out _start);
            LuaEnv.Global.Get("Update", out _update);
            LuaEnv.Global.Get("OnDestroy", out _onDestroy);
            LuaEnv.Global.Get("Test", out _test);
        }

        void Update()
        {
            if (_gameStarted == false || _update != null)
                _update();

            if ((Time.realtimeSinceStartup - _lastGCTime) > this.GCInterval)
            {
                _lastGCTime = Time.realtimeSinceStartup;
                LuaEnv.Tick();    
            }
        }

        void OnDestroy()
        {
            if (_gameStarted == false || _onDestroy != null)
                _onDestroy();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus == true)        
                LuaEnv.StopGc();
            else
                LuaEnv.RestartGc();
        }

    #if UNITY_DEBUG
        void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label(string.Format("LuaMemory: {0}KB", LuaEnv.Memroy));

            if (GUILayout.Button("Test", GUILayout.Width(100), GUILayout.Height(50)))
                this.DoTest();

            GUILayout.EndVertical();
        }
    #endif

        byte[] LoadBytes(string filepath)
        {
    #if !UNITY_DEBUG && !UNITY_EDITOR
            // Reload everytime in DEBUG, EDITOR mode.
            if (this._loadedLuas.ContainsKey(filepath))
            {
                return this._loadedLuas[filepath];
            }
    #endif

            var bytes = ResManager.Instance.LoadLua(filepath);
            if (bytes == null)
            {
                LogManager.Error(string.Format("Load lua file failure({0})", filepath));
                return new byte[]{};
            }

            var reloaded = this._loadedLuas.ContainsKey(filepath);
            // Cache
            if (this._loadedLuas.ContainsKey(filepath) == false)
            {
                _loadedLuas[filepath] = bytes;
            }

            // Force full gc when reloading lua file 
            if (reloaded) 
            {
                LogManager.Log(string.Format("Reload lua file successfully({0})", filepath));
                this.StartCoroutine(ForceFullGc());
            }
            else
                LogManager.Log(string.Format("Load lua file successfully({0})", filepath));

            return bytes;
        }

        /**
        * Load lua file and doString.
        * @filepath: Start from LuaRootPath without file extension.
        */
        public object Load(string filepath)
        {
            var bytes = this.LoadBytes(filepath);
            var objects = LuaEnv.DoString(bytes, filepath);
            return objects != null && objects.Length > 0 ? objects[0] : null;
        }

        IEnumerator ForceFullGc()
        {
            yield return new WaitForEndOfFrame();

            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();

            LuaEnv.Tick();
            LuaEnv.FullGc();
        }

        [ContextMenu("DoTest")]
        public void DoTest()
        {
            _test();
        }

        [ContextMenu("ReloadPrefix")]
        public void ReloadPrefix()
        {
            LuaEnv.DoString("include('Prefix')");
        }

        [ContextMenu("MemorySnapshot")]
        public void MemorySnapshot()
        {
            LuaEnv.DoString("LogManager.Log(Memory.snapshot())");
        }
    }
}

#endif