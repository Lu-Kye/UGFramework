using UGFramework.Utility;

namespace UGFramework.Res
{
    public static class ResConfig
    {
        public const string VERSION = "1.0.0";
        public const string VERSION_FILE = "resources_version.txt";

        public const string MAINIFEST = "res";

        public const string BUNDLE_EXTENSION = ".assetbundle";
        public const string LUA_EXTENSION = ".lua";
        public const string MOBILE_LUA_EXTENSION = ".txt";

        public const string RES_ROOT = "Res"; 
        public const string DEPENDENCIES_ROOT = "dependencies"; 
        public const string LUA_ROOT = "Lua"; 

#region UI
        public const string UI_ROOT = "UI";
        public static readonly string UI_PREFABS_ROOT = UI_ROOT + "/Prefabs";
        public static readonly string UI_TEXTURE = "Art/UI/Texture";
        public static readonly string UI_TEXTURE_RUNTIME_BUNDLE = "Runtime";
        public static readonly string UI_TEXTURE_RUNTIME = UI_TEXTURE + "/" + UI_TEXTURE_RUNTIME_BUNDLE;
#endregion


        // Resources server address
        public const string SERVER_URL = "http://192.168.10.127:8080";


        // When building application(ipa, apk), resources will be put in BUILDIN_PATH 
        public static string BUILDIN_PATH
        {
            get
            {
                return PathUtility.StreamingAssetsPath + "/" + ResConfig.RES_ROOT.ToLower();
            }
        }

        // When hot update resources at runtime, resources will be put in MOBILE_HOTUPDATE_PATH
        public static string MOBILE_HOTUPDATE_PATH
        {
            get
            {
    #if UNITY_EDITOR
                return PathUtility.PersistentPath + "/" + ResConfig.RES_ROOT.ToLower() + "_hotupdate";
    #else
                return PathUtility.PersistentPath + "/" + ResConfig.RES_ROOT.ToLower();
    #endif
            }
        }
    }
}