using System.IO;
using UnityEngine;
using System.Collections;
using UGFramework.Components;
using UGFramework.Utility;
using UGFramework.Log;

namespace UGFramework.Res
{
    public partial class ResManager : MonoBehaviour
    {
        public static ResManager Instance { get; private set; }
        ResManager() {}     

        WWWSyncAgent _wwwSyncAgent;

        public bool Simulate = false;

        void Awake()
        {
            Instance = this;
            _wwwSyncAgent = GameObjectUtility.GetComponent<WWWSyncAgent>(this.gameObject);
            _wwwSyncAgent.Timeout = 5f;
        }

        public void Init()
        {
            InitForAssetBundle();
            InitForLoader();
            InitForHotUpdate();
        }

        /**
        * If is editor, load lua file by fileStream.
        * If is mobile, load lua file by assetBundle.
        * @path : start from Assets(exclude), like "Scripts/*.cs"
        */
        public byte[] LoadLua(string path)
        {
            var fullpath = ResConfig.LUA_ROOT + "/" + path; byte[] bytes = null;

            // Editor
            if (Application.isMobilePlatform == false && this.Simulate == false)
            {
                fullpath = Application.dataPath + "/" + ResConfig.RES_ROOT + "/" + fullpath + ResConfig.LUA_EXTENSION;
                if (File.Exists(fullpath) == false)
                    return this._LoadLuaFromResources(path);
                return FileUtility.ReadFileBytes(fullpath);
            }

            // Mobile
            fullpath = (fullpath + ResConfig.MOBILE_LUA_EXTENSION).ToLower();
            this.LoadTxtAtPath(fullpath, ref bytes);
            if (bytes == null)
                return this._LoadLuaFromResources(path);
            return bytes;
        }
        byte[] _LoadLuaFromResources(string path)
        {
            var textAsset = Resources.Load<TextAsset>(path + ResConfig.LUA_EXTENSION);
            if (textAsset != null)
                return textAsset.bytes;
            return null;
        }

        // @return : scene path
        public string LoadScene(string sceneName)
        {
            string path = ResConfig.SCENE_ROOT + "/" + sceneName;
            // Editor
            if (Application.isMobilePlatform == false && this.Simulate == false)
            {
                path = ResConfig.RES_ROOT + "/" + path;
                return path;
            }

            // Mobile or Simulate
            path = path.ToLower() + ".unity";
            var loadedAssetBundle = this.LoadAssetBundle(path);
            if (loadedAssetBundle == null)
                return null;

            return loadedAssetBundle.AssetBundle.GetAllScenePaths()[0];
        }

        /**
        * If is editor, load asset by assetDatabase.
        * If is mobile, load asset by assetBundle.
        * @path : start from ResRoot(exclude), like "Lua/*.lua"
        */
        public T Load<T>(string path)
            where T : UnityEngine.Object
        {
            // Editor
            if (Application.isMobilePlatform == false && this.Simulate == false)
            {
    #if UNITY_EDITOR
                var fullpath = "Assets/" + ResConfig.RES_ROOT + "/" + path;
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullpath);
    #else
                return null;
    #endif
            }

            // Mobile
            return this.LoadFromCacheOrBundle<T>(path.ToLower());
        }

        /**
        * You must know what you are going to do, and be careful of unload resource
        */
        public void Unload(UnityEngine.Object assetToUnload)
        {
            this.StartCoroutine(_Unload(assetToUnload));
        }
        IEnumerator _Unload(UnityEngine.Object assetToUnload)
        {
            yield return new WaitForEndOfFrame();
            Resources.UnloadAsset(assetToUnload);
        }

        /**
        * Unload all unused assets, usually call this function when changing scene
        */
        public void UnloadUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}
