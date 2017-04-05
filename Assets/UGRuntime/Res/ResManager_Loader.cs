using System.Collections.Generic;
using System.IO;
using UGFramework.Log;
using UnityEngine;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        Dictionary<string, UnityEngine.Object> _cachedAssets = new Dictionary<string, UnityEngine.Object>();

        void InitForLoader()
        {
        }

        // Only load txt files
        string LoadTxtAtPath(string path, ref byte[] bytes)
        {
            var isHotUpdateAsset = false;
            // First try hot update path
            var fullpath = ResConfig.MOBILE_HOTUPDATE_PATH + "/" + path;
            if (File.Exists(fullpath))
            {
                isHotUpdateAsset = true;
            }
            else
            {
                // Second try buildin path
                LogManager.Log(string.Format("LoadAssetAtPath failure in MOBILE_HOTUPDATE_PATH({0})", fullpath));
                isHotUpdateAsset = false;
                fullpath = ResConfig.BUILDIN_PATH + "/" + path;
            }

            string txt = null;
            var url = Application.platform == RuntimePlatform.Android ? fullpath : "file://" + fullpath;

            using (var www = _wwwSyncAgent.LoadAsset(url))
            {
                if (www == null)
                {
                    LogManager.Error(string.Format("LoadAssetAtPath failure in BUILDIN_PATH({0})", fullpath));
                    return null;
                }
                if (isHotUpdateAsset == false)
                {
                    LogManager.Log(string.Format("LoadAssetAtPath successfully in BUILDIN_PATH({0})", fullpath));
                }
                else
                {
                    LogManager.Log(string.Format("LoadAssetAtPath successfully in MOBILE_HOTUPDATE_PATH({0})", fullpath));
                }

                txt = www.text;
                bytes = www.bytes;
            }
            return txt;
        }

        T LoadFromCacheOrBundle<T>(string path)
            where T : UnityEngine.Object
        {
            if (_cachedAssets.ContainsKey(path))
            {
                return _cachedAssets[path] as T;
            }
            
    #if UNITY_DEBUG
            var startTime = Time.realtimeSinceStartup;
    #endif

            var assetBundle = this.LoadAssetBundle(path);
            if (assetBundle == null)
                return null;

    #if UNITY_DEBUG
            LogManager.Log(string.Format("ResManager::LoadFromCacheOrBundle({0}) elapsed time:{1}", path, Time.realtimeSinceStartup - startTime));
    #endif

            var asset = _cachedAssets[path] = assetBundle.LoadAllAssets()[0];
            return asset as T;
        }

        // Before do test, build assetbundles at first (Guardian->Res->Build(IOS or other platforms))
        // Some simulate test cases
        [ContextMenu("TestLoadLuaInMobile(Simulate)")]
        public void TestLoadLuaInMobile()
        {
            byte[] bytes = null;
            var txt = this.LoadTxtAtPath("lua/main.txt", ref bytes);
            if (txt == null)
                LogManager.Log("txt is null");
            else
                LogManager.Log(txt);
        }
        [ContextMenu("TestLoadBundleInMobile(Simulate)")]
        public void TestLoadBundleInMobile()
        {
            var go = this.LoadFromCacheOrBundle<GameObject>("ui/prefabs/fullexample/fullexample.prefab");
            LogManager.Log(go.name);
        }
    }
}
