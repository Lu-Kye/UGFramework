using System.Collections.Generic;
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
        static readonly string ROOT = "";

        public static string Path { get; set; }

        public static object Object { get; private set; }

        /**
         * Call reset before drawing new root object
         */
        public static void Setup(object obj)
        {
            Object = obj;
            Path = ROOT;
        }

        static Dictionary<object, Dictionary<string, bool>> foldoutRecords = new Dictionary<object, Dictionary<string, bool>>();
        public static void AddFolder(string folder)
        {
            Path += folder + "/";
        }
        public static bool Foldout(GUIContent content)
        {
            if (foldoutRecords.ContainsKey(Object) == false)
                foldoutRecords[Object] = new Dictionary<string, bool>();
            if (foldoutRecords[Object].ContainsKey(Path) == false)
                foldoutRecords[Object][Path] = false;
            return foldoutRecords[Object][Path] = EditorGUILayout.Foldout(foldoutRecords[Object][Path], content, true);
        }
        public static void DrawTab()
        {
            int width = (Path.Split('/').Length - 1) * 1;
            EditorGUILayout.LabelField("", GUILayout.Width(width));
        }

        public static bool CheckMember(MemberInfo info)
        {
            return MemberDrawer.CheckMember(info);
        }
        public static bool DrawMember(MemberInfo info, GUIContent content = null)
        {
            return MemberDrawer.DrawMember(info, content);
        }

        public static bool DrawObject(object obj, GUIContent content = null)
        {
            return ObjectDrawer.Draw(obj, content);
        }

        public static bool DrawInt(ref int value, GUIContent content)
        {
            var nextValue = EditorGUILayout.IntField(content, value);
            var changed = value != nextValue;
            value = nextValue;
            return changed;
        }
    }
}