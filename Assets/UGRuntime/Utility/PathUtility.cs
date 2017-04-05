using UnityEngine;

namespace UGFramework.Utility
{
    public static class PathUtility
    {
        public static string StreamingAssetsPath 
        {
            get
            {
                return Application.streamingAssetsPath;
            }
        }    

        public static string PersistentPath
        {
            get
            {
                // Mobile
                if (Application.isMobilePlatform)
                {
                    return Application.persistentDataPath;
                }
                
                // Editor
                return Application.streamingAssetsPath;
            }
        }
    }
}
