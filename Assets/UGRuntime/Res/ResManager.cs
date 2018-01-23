using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UGFramework.Utility;
using UGFramework.Extension;
using UGFramework.Log;

namespace UGFramework.Res
{
    public partial class ResManager : MonoBehaviour
    {
        // Resources hotupdating/uncompress process info
        public struct ProcessInfo
        {
            // Current updating filepath
            public string File;

            // If error is empty meaning success
            public string Error;

            // Index of current updating file in updating files
            public int Index;

            // Count of updating files
            public int Count;

            public float Percent
            {
                get
                {
                    if (Count <= 0)
                        return 1f;
                    return (float)(Index + 1) / Count;
                }
            }
        }

        public static ResManager Instance { get; private set; }
        ResManager() {}     

        public bool Simulate = false;

        void Awake()
        {
            Instance = this;
        }

        void OnDestroy()
        {
#if UNITY_EDITOR
            // Delete hotupdate directory
            if (Directory.Exists(ResConfig.MOBILE_HOTUPDATE_PATH))
                Directory.Delete(ResConfig.MOBILE_HOTUPDATE_PATH, true);
#endif
        }

        public void Init()
        {
            this.InitForAssetBundle();
            this.InitForLoader();
            this.InitForHotUpdate();
            this.InitForCompress();
        }

        public void Reset()
        {
            this.ClearAssetBundles();
        }

        public byte[] LoadTxt(string fullpath)
        {
            var textAssets = this.Load<TextAsset>(fullpath);
            var bytes = new byte[textAssets.bytes.Length];
            textAssets.bytes.CopyTo(bytes, 0);
            this.UnloadAsset(fullpath);
            return bytes;
        }

        /**
         * If is editor, load lua file by fileStream.
         * If is mobile, load lua file by assetBundle.
         * @path : start from Assets(exclude), like "Scripts/*.cs"
         */
        public byte[] LoadLua(string path)
        {
            var fullpath = ResConfig.LUA_ROOT + "/" + path; 

            // Editor
            if (Application.isMobilePlatform == false && this.Simulate == false)
            {
                fullpath = Application.dataPath + "/" + ResConfig.RES_ROOT + "/" + fullpath + ResConfig.LUA_EXTENSION;
                return FileUtility.ReadFileBytes(fullpath);
            }

            // Mobile
            fullpath = fullpath + ResConfig.MOBILE_LUA_EXTENSION;
            return this.LoadTxt(fullpath);
        }

        // @path : contains file extension, like "bgBlack.png"(without "Runtime"), "IconBG/IconBG_1.png"
        public Sprite LoadSprite(string path)
        {
            var fileName = Path.GetFileName(path);
            var bundlePath = path.ReplaceLast(fileName, "");
            if (bundlePath.EndsWith("/")) bundlePath = bundlePath.ReplaceLast("/", "");
            var assetName = "Assets/" + ResConfig.UI_TEXTURE_RUNTIME + "/" + path;

            // Assets in runtime.bundle
            if (string.IsNullOrEmpty(bundlePath)) 
            {
                bundlePath = ResConfig.UI_TEXTURE_RUNTIME_BUNDLE;
                assetName = "Assets/" + ResConfig.UI_TEXTURE_RUNTIME + "/" + fileName;
                LogManager.Error(string.Format(
                    "Load sprite from runtime.assetbundle is obsoleted! sprite({0})",
                    assetName
                ));
                return null;
            }
            else
            {
                bundlePath = ResConfig.UI_TEXTURE_RUNTIME_BUNDLE + "/" + bundlePath;
            }
            path = ResConfig.UI_TEXTURE + "/" + bundlePath;

            var sprite = this.Load<Sprite>(path, assetName);
            return sprite;
        }
        public void UnloadSprite(string path)
        {
            var fileName = Path.GetFileName(path);
            var bundlePath = path.ReplaceLast(fileName, "");
            if (bundlePath.EndsWith("/")) bundlePath = bundlePath.ReplaceLast("/", "");
            var assetName = "Assets/" + ResConfig.UI_TEXTURE_RUNTIME + "/" + path;

            // Assets in runtime.bundle
            if (string.IsNullOrEmpty(bundlePath)) 
            {
                bundlePath = ResConfig.UI_TEXTURE_RUNTIME_BUNDLE;
                assetName = "Assets/" + ResConfig.UI_TEXTURE_RUNTIME + "/" + fileName;
                LogManager.Error(string.Format(
                    "Unload sprite from runtime.assetbundle is obsoleted! sprite({0})",
                    assetName
                ));
                return;
            }
            else
            {
                bundlePath = ResConfig.UI_TEXTURE_RUNTIME_BUNDLE + "/" + bundlePath;
            }
            path = ResConfig.UI_TEXTURE + "/" + bundlePath;
            this.UnloadAsset(path, assetName);
        }

        // @path : start from UIRoot(exclude), like "Login/Login(prefab)"
        public GameObject LoadUI(string path)
        {
            path = ResConfig.UI_PREFABS_ROOT + "/" + path + ".prefab";
            return this.Load<GameObject>(path);
        }
        public void UnloadUI(string path)
        {
            var bundlePathWithoutExtension = ResConfig.UI_PREFABS_ROOT + "/" + path + ".prefab";
            this.UnloadAsset(bundlePathWithoutExtension);
        }

        // @return : scene path
        public string LoadScene(string sceneName)
        {
            var path = ResConfig.SCENE_ROOT + "/" + sceneName;

            // Editor
            if (Application.isMobilePlatform == false && this.Simulate == false)
            {
                path = ResConfig.RES_ROOT + "/" + path;
                return path;
            }

            // Mobile or Simulate
            path = path + ".unity";
            var loadedAssetBundle = this.LoadAssetBundle(path);
            if (loadedAssetBundle == null)
                return null;

            return loadedAssetBundle.ScenePaths[0];
        }
        public void UnloadScene(string sceneName)
        {
            // Editor
            if (Application.isMobilePlatform == false && this.Simulate == false)
            {
                return;
            }
            // Mobile or Simulate
            var path = ResConfig.SCENE_ROOT + "/" + sceneName;
            path = path + ".unity";
            this.UnloadAssetBundle(path, false);
        }

        /**
         * If is editor, load asset by assetDatabase.
         * If is mobile, load asset by assetBundle.
         * @path : start from ResRoot(exclude), like "Lua/*.lua"
         */
        public T Load<T>(string path, string assetName = null)
            where T : UnityEngine.Object
        {
            var bundlePathWithoutExtension = path;
            // NOTE: Load asset from bundle will increase referenceCount of this bundle!
            var assetFromCacheOrBundle = this.LoadFromCacheOrBundle<T>(bundlePathWithoutExtension, assetName);
            return assetFromCacheOrBundle;
        }

        /**
         * You must know what you are going to do, and be careful of unloading asset
         * Note: @assetName is useless, cause unloading specific asset from an assetBundle is not implemented.
         */
        public void UnloadAsset(string bundlePathWithoutExtension, string assetName = null)
        {
            this.Unload(bundlePathWithoutExtension, assetName);
        }

        /**
         * Unload all unused assets, usually call this function when changing scene
         */
        Coroutine _unloadUnusedAssetCoroutine = null;
        public void UnloadUnusedAssets()
        {
            if (_unloadUnusedAssetCoroutine != null) 
                return;
            _unloadUnusedAssetCoroutine = this.StartCoroutine(_UnloadUnusedAssets());
        }
        IEnumerator _UnloadUnusedAssets()
        {
            yield return 1;
            yield return Resources.UnloadUnusedAssets();
            _unloadUnusedAssetCoroutine = null;
        }
    }
}