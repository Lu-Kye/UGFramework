using UnityEditor;

namespace UGFramework.Editor
{
    public static class PlatformUtility
    {
        public static bool Switch(BuildTarget targetPlatform)
        {
            var group = BuildTargetGroup.Unknown;
            switch (targetPlatform)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
                case BuildTarget.StandaloneOSXUniversal:
                    group = BuildTargetGroup.Standalone;
                    break;
                
                case BuildTarget.iOS:
                    group = BuildTargetGroup.iOS;
                    break;
                
                case BuildTarget.Android:
                    group = BuildTargetGroup.Android;
                    break;
            }
            return EditorUserBuildSettings.SwitchActiveBuildTarget(group, targetPlatform);
        }
    }
}