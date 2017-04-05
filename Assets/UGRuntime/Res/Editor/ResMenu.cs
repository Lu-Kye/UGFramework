using UGFramework.Editor;
using UnityEditor;

namespace UGFramework.Res
{
    public static class ResMenu
    {
        [MenuItem(TopbarConfig.RES + "/Build")]
        public static void Build()
        {
            ResBuildManager.Build(EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem(TopbarConfig.RES + "/BuildIOS")]
        public static void BuildIOS()
        {
            ResBuildManager.Build(BuildTarget.iOS);
        }

        [MenuItem(TopbarConfig.RES + "/BuildAndroid")]
        public static void BuildAndroid()
        {
            ResBuildManager.Build(BuildTarget.Android);
        }
        
        [MenuItem(TopbarConfig.RES + "/Clear")]
        public static void Clear()
        {
            ResBuildManager.Clear();
        }
    }
}