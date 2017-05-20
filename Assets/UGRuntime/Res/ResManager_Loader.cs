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

        void Unload(string path, bool unloadAllLoaded)
        {
            if (_cachedAssets.ContainsKey(path) == false)
                return;
            
            // Unload asset bundle
            this.UnloadAssetBundle(path, unloadAllLoaded);

            // Unload asset
            if (unloadAllLoaded)
            {
                var asset = _cachedAssets[path];
                Resources.UnloadAsset(asset);
                _cachedAssets.Remove(path);
            }
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

        T LoadFromCacheOrBundle<T>(string path, string assetName = null)
            where T : UnityEngine.Object
        {
            var bundlePath = path;
            var cachedAssetPath = string.IsNullOrEmpty(assetName) ? path : assetName;
            if (_cachedAssets.ContainsKey(cachedAssetPath))
            {
                if (_cachedAssets[cachedAssetPath] == null)
                    _cachedAssets.Remove(cachedAssetPath);
                else
                    return _cachedAssets[cachedAssetPath] as T;
            }
            
    #if UNITY_DEBUG
            var startTime = Time.realtimeSinceStartup;
    #endif

            var loadedAssetBundle = this.LoadAssetBundle(bundlePath);
            if (loadedAssetBundle == null) 
            {
                LogManager.Error("assetBundle is null:" + bundlePath);
                return null;
            }

    #if UNITY_DEBUG
            LogManager.Log(string.Format("ResManager::LoadFromCacheOrBundle({0}) elapsed time:{1}", path, Time.realtimeSinceStartup - startTime));
    #endif

            T asset = null;
            if (string.IsNullOrEmpty(assetName))
                asset = loadedAssetBundle.AssetBundle.LoadAllAssets<T>()[0];
            else
                asset = loadedAssetBundle.AssetBundle.LoadAsset<T>(assetName);
            _cachedAssets[cachedAssetPath] = asset;
            return asset;
        }
    }
}
