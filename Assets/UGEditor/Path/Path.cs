using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UGFramework.UGEditor
{
    public class Path 
    {
        public static string ProjectRoot
        {
            get
            {
                return Application.dataPath.Replace("Assets", "");
            }
        } 

        /**
         * UGFramework Assets path 
         */
        static string _uGRoot;
        public static string UGRoot
        {
            get
            {
#if UNITY_EDITOR
                if (string.IsNullOrEmpty(_uGRoot) == false)
                    return _uGRoot;

                var script = MonoScript.FromScriptableObject(new PathSeed());
                var scriptPath = AssetDatabase.GetAssetPath(script);
                return _uGRoot = ProjectRoot + scriptPath.Replace(scriptPath.Substring(scriptPath.LastIndexOf("/UGEditor/Path/")), "");
#else
                return _uGRoot;
#endif
            }
        }

        public static string UGEditorRoot = UGRoot + "/UGEditor";
    }
}