using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UGFramework.Log;

namespace UGFramework.Res
{
    public partial class ResManager
    {
    	public class LoadedAssetBundle
    	{
            string _bundleName;
            string _mainAssetName;
    
    		AssetBundle _bundle; 
    
    		public int ReferencedCount { get; set; }
    
            public string[] ScenePaths 
            {
                get
                {
                    return _bundle.GetAllScenePaths();
                }
            }
    
    		public LoadedAssetBundle(string path, string bundleName)
    		{
                _bundleName = bundleName.ToLower();
                _mainAssetName = path == ResConfig.MAINIFEST ? "AssetBundleManifest" : "Assets/" + ResConfig.RES_ROOT + "/" + path;
    
                // First try hot update path
                var fullpath = ResConfig.MOBILE_HOTUPDATE_PATH + "/" + _bundleName;
                if (File.Exists(fullpath) == false)
                {
                    // Second try buildin path
                    fullpath = ResConfig.BUILDIN_PATH + "/" + _bundleName;
                }
    
                // Editor
                if (Application.isMobilePlatform == false && ResManager.Instance.Simulate == false)
                {
                    return;
                }
    
    #if UNITY_EDITOR
                var time = Time.realtimeSinceStartup;
    #endif
    
                // Load assetBundle
                var assetBundle = AssetBundle.LoadFromFile(fullpath);
                _bundle = assetBundle;
                if (_bundle == null)
                {
                    LogManager.Error(string.Format(
                        "ResManager.LoadedAssetBundle:Ctor failure, bundle({0}) not found!", 
                        fullpath
                    ));
                }
    
    #if UNITY_EDITOR
                if (ResManager.Instance.ShowElapsedTime)
                {
                    var elapsedTime = (Time.realtimeSinceStartup - time) * 1000;
                    if (elapsedTime > 50)
                    {
                        LogManager.Error(string.Format(
                            "Load assetBundle({0}) elapsedTime:({1:###.#}ms)",
                            Path.GetFileName(fullpath),
                            elapsedTime
                        ), this);
                    }
                }
    #endif
            }

            Dictionary<string, UnityEngine.Object> _assets = new Dictionary<string, UnityEngine.Object>();
    
            public T LoadAsset<T>(string assetName)
                where T : UnityEngine.Object
            {
                if (_assets.ContainsKey(assetName))
                    return _assets[assetName] as T;

                UnityEngine.Object asset = null;
    
                // Editor
    #if UNITY_EDITOR
                if (Application.isMobilePlatform == false && ResManager.Instance.Simulate == false)
                {
                    asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetName);
                    if (asset == null)
                    {
                        LogManager.Error(string.Format(
                            "LoadAsset<T> load from AssetDatabase error! asset({0})", 
                            assetName
                        ), this);
                    }                
                    _assets[assetName] = asset;
                    return asset as T;
                }
    #endif
    
                // Mobile
                if (_bundle == null) 
                {
                    LogManager.Error(string.Format(
                        "LoadAsset<T> error, bundle is nil! asset({0})", 
                        assetName
                    ), this);
                    return null;
                }

                // Check 
                var assetNameLower = assetName.ToLower();
                if (_bundle.Contains(assetNameLower) == false)
                {
                    LogManager.Error(string.Format(
                        "LoadAsset<T> error, asset does not exist! asset({0})", 
                        assetNameLower
                    ), this);
                    return null;
                }
    
                // Load by assetBundle
                asset = _bundle.LoadAsset<T>(assetNameLower);
                if (asset == null) 
                {
                    LogManager.Error(string.Format(
                        "LoadAsset<T> load asset frome bundle error! asset({0})", 
                        assetName
                    ), this);
                    return null;
                }
    
    #if UNITY_EDITOR
                // Load in editor by assetDatabase 
                // and override asset in assetBundle(only for prefab(because of loading animated prefab error))
                var assetBundleName = _bundleName;
                while (asset is GameObject)
                {
                    var go = asset as GameObject;
                    var animators = go.GetComponentsInChildren<Animator>();
                    if (animators == null || animators.Length <= 0)
                        break;
    
                    var assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
                    if (assetPaths.Length == 0)
                    {
                        LogManager.Error(string.Format(
                            "LoadAssetBundle ctor error, assetBundle not found in Editor! assetBundleName({0}) assetName({1})",
                            assetBundleName,
                            assetName
                        ));
                    }
                    else
                    {
                        var index = -1;
                        for (int i = 0; i < assetPaths.Length; ++i)
                        {
                            var assetPath = assetPaths[i];
                            var assetName1 = Path.GetFileName(assetName).ToLower();
                            var assetName2 = Path.GetFileName(assetPath).ToLower();
                            if (assetName1 == assetName2)
                            {
                                index = i;
                                break;
                            }
                        }
                        if (index == -1)
                        {
                            LogManager.Error(string.Format(
                                "LoadAssetBundle ctor error, assetBundle index not found in Editor! assetBundleName({0}) assetName({1})",
                                assetBundleName,
                                assetName
                            ));
                        }
                        asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPaths[index]);
                    }
                    break;
                }
    #endif

                _assets[assetName] = asset;
                return asset as T;
            }
    
            public T LoadMainAsset<T>()
                where T : UnityEngine.Object
            {
                return this.LoadAsset<T>(_mainAssetName);    
            }
    
            public void Unload()
            {
                if (_bundle != null)
                {
                    _bundle.Unload(false);
                    UnityEngine.Object.Destroy(_bundle);
                    _bundle = null;
                }
            }
    	}
    
        Dictionary<string, LoadedAssetBundle> _assetBundles = new Dictionary<string, LoadedAssetBundle>();
        Dictionary<string, string[]> _dependenceInfos = new Dictionary<string, string[]>();
    
        AssetBundleManifest _manifest;
        bool _manifestLoading = false;
    
        void InitForAssetBundle()
        {
        }
    
        void EnsureManifest()
        {
            if (Application.isMobilePlatform == false && this.Simulate == false)
                return;
            
            if (_manifest != null || _manifestLoading)
                return;
            _manifestLoading = true;
    
            // Clear at first    
            this.ClearAssetBundles();
    
            // Init manifest
            var manifestAssetBundle = this.LoadAssetBundle(ResConfig.MAINIFEST);
            if (manifestAssetBundle == null)
            {
                LogManager.Error("ResManager::InitForAssetBundle error, manifest bundle is null!");
                return;
            }
            _manifest = manifestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            _manifestLoading = false;
            if (_manifest == null)
            {
                LogManager.Error("ResManager::InitForAssetBundle error, manifest is null!");
            }
        }
    
        void ClearAssetBundles()
        {
            var paths = _assetBundles.Keys.ToArray();
            for (int i = 0; i < paths.Length; ++i)
            {
                var path = paths[i];
                if (path == ResConfig.MAINIFEST)
                    continue;
                this.UnloadAssetBundle(path, true);
            }
            this.UnloadAssetBundle(ResConfig.MAINIFEST, true);
            _manifest = null;
            _assetBundles.Clear();
        }
    
        LoadedAssetBundle TryGetLoadedAssetBundle(string path)
        {
            LoadedAssetBundle loadedAssetBundle = null;
    		if (_assetBundles.TryGetValue(path, out loadedAssetBundle))
            {
                return loadedAssetBundle;
            }
            return null;
        }
    
        bool UnloadAssetBundle(string path, bool force = false)
        {
            var cachePath = path.ToLower();
            var loadedAssetBundle = this.TryGetLoadedAssetBundle(cachePath);
    		if (loadedAssetBundle == null)
            {
                return false;
            }
            
            var isManifest = path == ResConfig.MAINIFEST;
            var bundleName = isManifest ? path : ResConfig.ConvertToBundleName(path) + ResConfig.BUNDLE_EXTENSION;
    
            // Try unload bundle assets, and remove from cache
            if (force)
                loadedAssetBundle.ReferencedCount = 0;
            else
                loadedAssetBundle.ReferencedCount--;
    
            // Check referencedCount
            var unload = false;
            if (loadedAssetBundle.ReferencedCount <= 0)
            {
                unload = true;
                loadedAssetBundle.Unload();
                _assetBundles.Remove(cachePath);
                this.DebugDeallocBundle(cachePath);
                if (isManifest)
                    _manifest = null;
            }
    
            // Try unload depenceBundle
            if (unload == true && _dependenceInfos.ContainsKey(bundleName))
            {
                var dependencies = _dependenceInfos[bundleName];
                for (var i = 0; i < dependencies.Length; ++i)
                {
                    var dependencePath = ResConfig.ReverseFromBundleName(dependencies[i]);
                    this.UnloadAssetBundle(dependencePath, force);
                }
                _dependenceInfos.Remove(bundleName);
            }
    
            return true;
        }
    
        LoadedAssetBundle LoadAssetBundle(string path, bool isDependence = false)        
        {
            this.EnsureManifest();
            var cachePath = path.ToLower();
            var loadedAssetBundle = this.TryGetLoadedAssetBundle(cachePath);
    		if (loadedAssetBundle != null)
            {
                loadedAssetBundle.ReferencedCount++;
                return loadedAssetBundle;
            }
    
            var isManifest = path == ResConfig.MAINIFEST;
            var bundleName = isManifest ? path : ResConfig.ConvertToBundleName(path) + ResConfig.BUNDLE_EXTENSION;
    
            // Load dependencies
            if (isDependence == false)
            {
                var dependencies = this.GetDependencies(bundleName, path);
                for (var i = 0; i < dependencies.Length; ++i)
                {
                    var dependencePath = ResConfig.ReverseFromBundleName(dependencies[i]);
                    var dependenceBundle = this.LoadAssetBundle(dependencePath, true);
                    if (dependenceBundle == null)
                    {
                        LogManager.Error(string.Format(
                            "ResManager::LoadAssetFromBundle error, dependence({0}) is null!", 
                            dependencePath
                        ));
                    }
                }
            }
    
            // Load & cache assetBundle
            loadedAssetBundle = new LoadedAssetBundle(path, bundleName);
            loadedAssetBundle.ReferencedCount = 1;
            _assetBundles.Add(cachePath, loadedAssetBundle);
            this.DebugAllocBundle(cachePath);
            
            return loadedAssetBundle;
        }
    }
}