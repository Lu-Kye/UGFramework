using UnityEditor;

namespace UGFramework.UGEditor.Inspector
{
    public static class InspectorMenu
    {
        [MenuItem(MenuConfig.INSPECTOR + "/Expand")]
        public static void Expand()    
        {
            InspectorUtility.Foldout(true, null, InspectorUtility.SelectedPath);
        }

        [MenuItem(MenuConfig.INSPECTOR + "/ExpandAll")]
        public static void ExpandAll()    
        {
            InspectorUtility.Foldout(true, null, InspectorUtility.SelectedPath, true);
        }

        [MenuItem(MenuConfig.INSPECTOR + "/Collapse")]
        public static void Collapse()    
        {
            InspectorUtility.Foldout(false, null, InspectorUtility.SelectedPath);
        }

        [MenuItem(MenuConfig.INSPECTOR + "/CollapseAll")]
        public static void CollapseAll()    
        {
            InspectorUtility.Foldout(false, null, InspectorUtility.SelectedPath, true);
        }
    }
}