using System.IO;
using UnityEngine;
using UGFramework.Extension;
using UGFramework.Utility;

namespace UGFramework.Res
{
    public class ResConfig
    {
        public static string VERSION = "1.0.0";
        public const string VERSION_FILE = "resources_version.txt";

        public const string MAINIFEST = "res";

#region File Extension
        public const string BUNDLE_EXTENSION = ".assetbundle";
        public const string BUNDLE_MANIFEST_EXTENSION = ".assetbundle.manifest";
        public const string LUA_EXTENSION = ".lua";
        public const string MOBILE_LUA_EXTENSION = ".txt";
#endregion

        public const string RES_ROOT = "Res"; 
        public const string DEPENDENCIES_ROOT = "dependencies"; 
        public const string LUA_ROOT = "Lua"; 
        public const string SCENE_ROOT = "Scene";
        public const string LEVEL_ROOT = "Levels";
        public const string TILE_ROOT = "Tile";
        public const string ROLE_ROOT = "Role";
        public const string MAP_ROOT = "Map";
    	public const string RESPRELOAD_ROOT = "ResPreLoad";
        public static readonly string FX_PREFABS_ROOT = "FX/Prefabs";
        public static readonly string MATERIALS_ROOT = "Materials";

#region UI
        public const string UI_ROOT = "UI";
        public static readonly string UI_PREFABS_ROOT = UI_ROOT + "/Prefabs";
        public static readonly string UI_TEXTURE = "Art/UI/Texture";
        public static readonly string UI_TEXTURE_RUNTIME_BUNDLE = "Runtime";
        public static readonly string UI_TEXTURE_RUNTIME = UI_TEXTURE + "/" + UI_TEXTURE_RUNTIME_BUNDLE;
#endregion

        // Compress assetbundles or not
        public static bool ENABLE_COMPRESS = true;

        // Resources server address
        public static string SERVER_URL = "http://localhost:8080";
        public static string PLATFORM_SERVER_URL 
        {
            get 
            {
                return SERVER_URL + "/" + ResConfig.PLATFORM_PREFIX_PATH + "/" + ResConfig.RES_ROOT.ToLower(); 
            }
        }

        // When building application(ipa, apk), resources will be put in BUILDIN_PATH 
        public static string BUILDIN_PATH
        {
            get
            {
                return PathUtility.StreamingAssetsPath + "/" + ResConfig.PLATFORM_PREFIX_PATH + "/" + ResConfig.RES_ROOT.ToLower();
            }
        }

        // When hot update resources at runtime, resources will be put in MOBILE_HOTUPDATE_PATH
        public static string MOBILE_HOTUPDATE_PATH
        {
            get
            {
#if UNITY_EDITOR
                return PathUtility.PersistentPath + "/" + ResConfig.PLATFORM_PREFIX_PATH + "/" + ResConfig.RES_ROOT.ToLower() + "_hotupdate";
#else
                return PathUtility.PersistentPath + "/" + ResConfig.PLATFORM_PREFIX_PATH + "/" + ResConfig.RES_ROOT.ToLower();
#endif
            }
        }

        public const string PLATFORM_PREFIX_ANDROID = "android";
        public const string PLATFORM_PREFIX_IOS = "ios";
        public const string PLATFORM_PREFIX_WIN = "windows";
        public const string PLATFORM_PREFIX_OSX = "osx";

        static string PLATFORM_PREFIX_PATH
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                    return PLATFORM_PREFIX_ANDROID;
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return PLATFORM_PREFIX_IOS;
                else if (Application.platform == RuntimePlatform.WindowsEditor)
                    return PLATFORM_PREFIX_WIN;
                else if (Application.platform == RuntimePlatform.OSXEditor) 
                    return PLATFORM_PREFIX_OSX;
                return "unknown";
            }
        }

        // @return : asset fullpath with extension, eg: "art/ui/texture/texture1.png"
        public static string ConvertToBundleName(string assetFullpathWithExtension)
        {
            var extension = Path.GetExtension(assetFullpathWithExtension);
            var bundleName = assetFullpathWithExtension.ReplaceLast(extension, extension.ReplaceFirst(".", "__ext__")); 
            bundleName = bundleName.Replace(" ", "__space__");
            return bundleName;
        }
        public static string ReverseFromBundleName(string bundleName)
        {
            bundleName = bundleName.ReplaceLast(ResConfig.BUNDLE_EXTENSION, "");
            bundleName = bundleName.Replace("__space__", " ");
            var path = bundleName.ReplaceLast("__ext__", ".");
            return path;
        }

        public static string[] DONT_COMPRESS_EXTESIONS = new string[]
        {
            ".manifest",
            ResConfig.ConvertToBundleName(ResConfig.MAINIFEST),
            ResConfig.ConvertToBundleName(ResConfig.VERSION_FILE),
        };
        public static bool IsCompressable(string assetFullpathWithExtension)
        {
            if (ResConfig.ENABLE_COMPRESS == false)
                return false;    
            var file = assetFullpathWithExtension.EndsWith(ResConfig.BUNDLE_EXTENSION) ? assetFullpathWithExtension.ReplaceLast(ResConfig.BUNDLE_EXTENSION, "") : assetFullpathWithExtension;
            for (var i = 0; i < DONT_COMPRESS_EXTESIONS.Length; ++i)
            {
                if (file.EndsWith(DONT_COMPRESS_EXTESIONS[i]))
                {
                    return false;
                }
            }
            return true;
        }

#region Build
        static string[] BUNDLENAME_IS_FOLDER = new string[]
        {
            "Assets/" + ResConfig.UI_TEXTURE,
            "Assets/Art/Models/Scene_Desert_01",
            "Assets/Art/Models/Scene_Home_01",
            "Assets/Art/Models/Scene_Home_02",
            "Assets/Art/Models/Scene_Redonda_01",
            "Assets/AI",
            "Assets/Art/Animations/FBX/Hero",
            "Assets/Art/FX/Texture",
        };
        static string[] EXCLUDE_BUNDLENAME_IS_FOLDER = new string[]
        {
        };
        public static bool IsFolderAsBundleName(string bundlePath)
        {
            for (var i = 0; i < BUNDLENAME_IS_FOLDER.Length; ++i)
            {
                var includeConfig = BUNDLENAME_IS_FOLDER[i];
                if (bundlePath.StartsWith(includeConfig))
                {
                    var exclude = false;
                    for (var j = 0; j < EXCLUDE_BUNDLENAME_IS_FOLDER.Length; ++j)
                    {
                        var excludeConfig = EXCLUDE_BUNDLENAME_IS_FOLDER[j];
                        if (bundlePath.StartsWith(excludeConfig))
                        {
                            exclude = true;
                            break;
                        }
                    }
                    return exclude == false;
                }
            }
            return false;
        }
#endregion
    }
}