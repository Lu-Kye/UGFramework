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
            path = ResConfig.LUA_ROOT + "/" + path;
            byte[] bytes = null;

            // Editor
            if (Application.isMobilePlatform == false && this.Simulate == false)
            {
                path = Application.dataPath + "/" + ResConfig.RES_ROOT + "/" + path + ResConfig.EDITOR_LUA_EXTENSION;
                if (File.Exists(path) == false)
                {
                    LogManager.Warning(string.Format("Lua file not exists({0})", path));
                    return bytes;
                }
                return FileUtility.ReadFileBytes(path);
            }

            // Mobile
            path = (path + ResConfig.MOBILE_LUA_EXTENSION).ToLower();
            this.LoadTxtAtPath(path, ref bytes);
            return bytes;
        }

        // @path : start from UIRoot(exclude), like "Login/Login(prefab)"
        public GameObject LoadUI(string path)
        {
            path = ResConfig.UI_ROOT + "/" + path + ".prefab";
            return this.Load<GameObject>(path);
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
