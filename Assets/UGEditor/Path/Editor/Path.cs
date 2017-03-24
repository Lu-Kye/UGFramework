using UnityEngine;
using UnityEditor;

namespace UGFramework.Editor
{
    public class Path 
    {
        public static string ProjectRoot = Application.dataPath.Replace("Assets", "");

        /**
         * UGFramework Assets path 
         */
        static string _uGRoot;
        public static string UGRoot
        {
            get
            {
                if (string.IsNullOrEmpty(_uGRoot) == false)
                    return _uGRoot;

                var script = MonoScript.FromScriptableObject(new PathSeed());
                var scriptPath = AssetDatabase.GetAssetPath(script);
                return _uGRoot = ProjectRoot + scriptPath.Replace(scriptPath.Substring(scriptPath.LastIndexOf("/UGEditor/Path/")), "");
            }
        }

        public static string UGEditorRoot = UGRoot + "/UGEditor";
    }
}