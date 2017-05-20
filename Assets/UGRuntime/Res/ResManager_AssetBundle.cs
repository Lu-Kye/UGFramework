using System.Collections.Generic;
using System.IO;
using UGFramework.Extension.String;
using UGFramework.Log;
using UnityEngine;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        public class LoadedAssetBundle
        {
            public AssetBundle AssetBundle { get; private set; }
            public int ReferencedCount { get; set; }
            public bool IsDependence { get; set; }
            
            public LoadedAssetBundle(AssetBundle assetBundle)
            {
                this.AssetBundle = assetBundle;
            }
        }

        Dictionary<string, LoadedAssetBundle> _assetBundles = new Dictionary<string, LoadedAssetBundle>();
        AssetBundleManifest _manifest;

        void InitForAssetBundle()
        {
            if (Application.isMobilePlatform == false && this.Simulate == false)
                return;

            _assetBundles.Clear();

            // Init manifest
            var manifestAssetBundle = this.LoadAssetBundle(ResConfig.MAINIFEST);
            if (manifestAssetBundle == null)
            {
                LogManager.Error("ResManager::InitForAssetBundle error, manifest bundle is null!");
                return;
            }
            _manifest = manifestAssetBundle.AssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (_manifest == null)
            {
                LogManager.Error("ResManager::InitForAssetBundle error, manifest is null!");
            }
        }

        void UnloadAssetBundle(string path, bool unloadAllLoadedObject = false)
        {
            LoadedAssetBundle loadedAssetBundle = null;
            if (_assetBundles.TryGetValue(path, out loadedAssetBundle) == false)
                return;
            
            var isManifest = path == ResConfig.MAINIFEST;
            var bundleName = isManifest ? path : path + ResConfig.BUNDLE_EXTENSION;

            // Not support unload manifest bundle
            if (isManifest)
                return;
            
            // Unload bundle assets, and remove from cache
            if (unloadAllLoadedObject)
            {
                loadedAssetBundle.AssetBundle.Unload(true);
                _assetBundles.Remove(path);
            }
            else
            {
                loadedAssetBundle.AssetBundle.Unload(false);
            }

            // Try unload depenceBundle
            var dependencies = _manifest.GetAllDependencies(bundleName);
            for (var i = 0; i < dependencies.Length; ++i)
            {
                var dependence = dependencies[i].Replace(ResConfig.BUNDLE_EXTENSION, "");
                LoadedAssetBundle dependenceAssetBundle = null;
                if (_assetBundles.TryGetValue(dependence, out dependenceAssetBundle) == false)
                    continue;
                if (dependenceAssetBundle.IsDependence == false)
                    continue;
                
                if (unloadAllLoadedObject)
                {
                    // Cut down reference count
                    dependenceAssetBundle.ReferencedCount--;

                    // Unload
                    if (dependenceAssetBundle.ReferencedCount <= 0)
                    {
                        dependenceAssetBundle.AssetBundle.Unload(true);
                        _assetBundles.Remove(dependence);
                    }
                }
                else
                {
                    dependenceAssetBundle.AssetBundle.Unload(false);
                }
            }
        }

        LoadedAssetBundle LoadAssetBundle(string path)        
        {
            LoadedAssetBundle loadedAssetBundle = null;
            if (_assetBundles.TryGetValue(path, out loadedAssetBundle))
            {
                loadedAssetBundle.IsDependence = false;
                return loadedAssetBundle;
            }

            // Make sure to remove unloaded bundle from cache
            _assetBundles.Remove(path);

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

            var assetBundle = AssetBundle.LoadFromFile(fullpath);
            if (assetBundle == null)
            {
                LogManager.Log(string.Format("ResManager::LoadAssetFromBundle failure in BUILDIN_PATH({0})", fullpath));
                return null;
            }

            loadedAssetBundle = new LoadedAssetBundle(assetBundle);
            loadedAssetBundle.IsDependence = false;
            // Cache assetBundle
            _assetBundles.Add(path, loadedAssetBundle);

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
                    var dependence = dependencies[i].Replace(ResConfig.BUNDLE_EXTENSION, "");
                    var dependenceBundle = this.LoadAssetBundle(dependence);
                    dependenceBundle.IsDependence = true;
                    if (dependenceBundle == null)
                    {
                        LogManager.Error(string.Format("ResManager::LoadAssetFromBundle error, dependence({0}) is null!", dependence));
                    }
                    else
                    {
                        LogManager.Log(string.Format("ResManager::LoadAssetFromBundle load dependence successfully dependence({0})!", dependence));
                        dependenceBundle.ReferencedCount++;
                    }
                }
            }
            return loadedAssetBundle;
        }
    }
}
