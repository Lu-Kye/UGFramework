using UnityEditor;

namespace UGFramework.Editor.Inspector
{
    public static class InspectorMenu
    {
        [MenuItem(TopbarConfig.INSPECTOR + "/Expand")]
        public static void Expand()    
        {
            InspectorUtility.Foldout(true, null, InspectorUtility.SelectedPath);
        }

        [MenuItem(TopbarConfig.INSPECTOR + "/ExpandAll")]
        public static void ExpandAll()    
        {
            InspectorUtility.Foldout(true, null, InspectorUtility.SelectedPath, true);
        }

        [MenuItem(TopbarConfig.INSPECTOR + "/Collapse")]
        public static void Collapse()    
        {
            InspectorUtility.Foldout(false, null, InspectorUtility.SelectedPath);
        }

        [MenuItem(TopbarConfig.INSPECTOR + "/CollapseAll")]
        public static void CollapseAll()    
        {
            InspectorUtility.Foldout(false, null, InspectorUtility.SelectedPath, true);
        }
    }
}