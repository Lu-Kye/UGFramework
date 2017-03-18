using UnityEngine;
using UnityEditor;

namespace UGFramework.Editor.Inspector
{
    /**
     * --- DOC BEGIN ---
     * Custom inspector utility, draw fields in *Inspector*
     * --- DOC END ---
     */
    public static class InspectorUtility
    {
        public static bool DrawObject(object obj, GUIContent content = null)
        {
            return ObjectDrawer.Draw(obj, content);
        }

        public static bool DrawMember(MemberInfo info)
        {
            if (info.Type == typeof(int))
            {
                var value = info.GetInt();
                var changed = DrawInt(ref value, new GUIContent(info.Name));
            }

            return false;
        }

        public static bool DrawInt(ref int value, GUIContent content)
        {
            EditorGUILayout.IntField(content, value);
            return false;
        }
    }
}