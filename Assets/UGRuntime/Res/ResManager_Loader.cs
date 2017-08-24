using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UGFramework.Log;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        public bool ShowElapsedTime = false;
    
        void InitForLoader()
        {
        }
    
        void Unload(string path, string assetName = null)
        {
            var bundlePathWithoutExtension = path;
    
            // Try unload asset bundle
            if (this.UnloadAssetBundle(bundlePathWithoutExtension) == false) 
            {
    #if UNITY_EDITOR
    			LogManager.Error(string.Format(
                    "Unload error, path({0}) unloaded error! Path error or Somewhere unload asset mutliple times!!!",
                    path
                ));
    #endif
            }
        }
    
        T LoadFromCacheOrBundle<T>(string path, string assetName = null)
            where T : UnityEngine.Object
        {
            var bundlePathWithoutExtension = path;
            Object asset = null;
    
    #if UNITY_EDITOR
            var startTime = Time.realtimeSinceStartup;
    #endif
    
            var loadedAssetBundle = this.LoadAssetBundle(bundlePathWithoutExtension);
    		if (loadedAssetBundle == null) 
            {
    			LogManager.Error("assetBundle is null:" + bundlePathWithoutExtension);
    			return null;
    		}
    
    #if UNITY_EDITOR
            if (this.ShowElapsedTime)
            {
                var elapsedTime = (Time.realtimeSinceStartup - startTime) * 1000;
                if (elapsedTime >= 50)
                {
                    LogManager.Error(string.Format(
                        "LoadFromCacheOrBundle({0}) elapsedTime:({1}ms)",
                        path, 
                        elapsedTime
                    ), this);
                }
            }
    #endif
    
            if (string.IsNullOrEmpty(assetName))
                asset = loadedAssetBundle.LoadMainAsset<T>();
            else
                asset = loadedAssetBundle.LoadAsset<T>(assetName);
    
            return asset as T;
        }
    }
}