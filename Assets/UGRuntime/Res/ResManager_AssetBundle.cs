using System.Collections.Generic;
using System.IO;
using UGFramework.Log;
using UnityEngine;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        Dictionary<string, AssetBundle> _cachedAssetBundles = new Dictionary<string, AssetBundle>();
        AssetBundleManifest _manifest;

        void InitForAssetBundle()
        {
            if (Application.isMobilePlatform == false && this.Simulate == false)
                return;

            _cachedAssetBundles.Clear();

            // Init manifest
            var manifestAssetBundle = this.LoadAssetBundle(ResConfig.MAINIFEST);
            if (manifestAssetBundle == null)
            {
                LogManager.Error("ResManager::InitForAssetBundle error, manifest bundle is null!");
                return;
            }
            _manifest = manifestAssetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            if (_manifest == null)
            {
                LogManager.Error("ResManager::InitForAssetBundle error, manifest is null!");
            }
        }

        AssetBundle LoadAssetBundle(string path)        
        {
            AssetBundle assetBundle;
            if (_cachedAssetBundles.TryGetValue(path, out assetBundle))
                return assetBundle;

            var isManifest = path == ResConfig.MAINIFEST;
            var bundleName = isManifest ? path : path + ResConfig.BUNDLE_EXTENSION;

            var isHotUpdate = false;
            // First try hot update path
            var fullpath = ResConfig.MOBILE_HOTUPDATE_PATH + "/" + bundleName;
            if (File.Exists(fullpath))
            {
                isHotUpdate = true;
            }
            else
            {
                // Second try buildin path
                LogManager.Log(string.Format("ResManager::LoadAssetFromBundle failure in MOBILE_HOTUPDATE_PATH({0})", fullpath));
                isHotUpdate = false;
                fullpath = ResConfig.BUILDIN_PATH + "/" + bundleName;
            }

            assetBundle = AssetBundle.LoadFromFile(fullpath);
            if (assetBundle == null)
            {
                LogManager.Log(string.Format("ResManager::LoadAssetFromBundle failure in BUILDIN_PATH({0})", fullpath));
                return null;
            }
            // Cache assetBundle
            _cachedAssetBundles.Add(path, assetBundle);

            if (isHotUpdate == false)
                LogManager.Log(string.Format("ResManager::LoadAssetFromBundle successfully in BUILDIN_PATH({0})", fullpath));
            else
                LogManager.Log(string.Format("ResManager::LoadAssetFromBundle successfully in MOBILE_HOTUPDATE_PATH({0})", fullpath));
            
            // Load dependencies
            if (isManifest == false && _manifest != null)
            {
                var dependencies = _manifest.GetAllDependencies(bundleName);
                for (var i = 0; i < dependencies.Length; ++i)
                {
                    var dependence = dependencies[i].ReplaceLast(ResConfig.BUNDLE_EXTENSION, "");
                    AssetBundle dependenceBundle;
                    // Is already loaded?
                    if (_cachedAssetBundles.TryGetValue(dependence, out dependenceBundle))
                        continue;

                    dependenceBundle = LoadAssetBundle(dependence);
                    if (dependenceBundle != null)
                        dependenceBundle.LoadAllAssets();
                    else
                        LogManager.Error(string.Format("ResManager::LoadAssetFromBundle error, dependence({0}) is null!", dependence));
                }
            }

            return assetBundle;
        }
    }
}
