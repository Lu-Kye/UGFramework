using UGFramework.UGEditor;
using UnityEditor;

namespace UGFramework.Res
{
    public static class ResMenu
    {
        [MenuItem(MenuConfig.RES + "/BuildIOS")]
        public static void BuildIOS()
        {
            ResBuildManager.Build(BuildTarget.iOS);
        }
    
        [MenuItem(MenuConfig.RES + "/BuildAndroid")]
        public static void BuildAndroid()
        {
            ResBuildManager.Build(BuildTarget.Android);
        }
    
        [MenuItem(MenuConfig.RES + "/BuildWin")]
        public static void BuildWin()
        {
            ResBuildManager.Build(BuildTarget.StandaloneWindows64);
        }
    
        [MenuItem(MenuConfig.RES + "/BuildOSX")]
        public static void BuildOSX()
        {
            ResBuildManager.Build(BuildTarget.StandaloneOSXUniversal);
        }
        
        [MenuItem(MenuConfig.RES + "/Clear")]
        public static void Clear()
        {
            ResBuildManager.Clear();
        }
    }
}