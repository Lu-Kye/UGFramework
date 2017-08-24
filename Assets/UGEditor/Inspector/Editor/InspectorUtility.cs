using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

namespace UGFramework.UGEditor.Inspector
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

        public static string SelectedPath { get; private set; }

        // If true, some readonly type will recognize as writeable. eg: Dictionary, LinkedList .etc
        public static bool CustomSerialize { get; set; }

        /**
         * Call reset before drawing new root object
         */
        public static void Setup(object obj, bool customSerialize = false)
        {
            Object = obj;
            Path = ROOT;
            CustomSerialize = customSerialize; 
        }

        static Dictionary<object, Dictionary<string, bool>> foldoutRecords = new Dictionary<object, Dictionary<string, bool>>();
        public static void AddFolder(string folder)
        {
            Path += folder + "/";
        }
        public static void Foldout(bool foldout, object obj = null, string path = null, bool recursive = false)
        {
            obj = obj == null ? Object : obj;
            path = string.IsNullOrEmpty(path) ? Path : path;
            foldoutRecords[obj][path] = foldout;
            if (recursive)
            {
                var tmpPaths = new string[foldoutRecords[obj].Keys.Count];
                foldoutRecords[obj].Keys.CopyTo(tmpPaths, 0);
                foreach (var tmpPath in tmpPaths)
                {
                    if (tmpPath.StartsWith(path) == false)
                        continue;
                    foldoutRecords[obj][tmpPath] = foldout;
                }
            }
        }
        public static bool ForceFoldout { get; set; }
        public static bool DrawFoldout(GUIContent content, bool defaultFoldout = false)
        {
            if (foldoutRecords.ContainsKey(Object) == false)
                foldoutRecords[Object] = new Dictionary<string, bool>();
            if (foldoutRecords[Object].ContainsKey(Path) == false)
            {
                foldoutRecords[Object][Path] = defaultFoldout;
            }
            
            foldoutRecords[Object][Path] = EditorGUILayout.Foldout(foldoutRecords[Object][Path], content, true);
            // Debug.Log("DrawFoldout " + Path);

            var contentRect = GUILayoutUtility.GetLastRect();
            var evt = Event.current;
            var mousePos = evt.mousePosition;
            if (evt.type == EventType.ContextClick && contentRect.Contains(mousePos)) {
                SelectedPath = Path;
                EditorUtility.DisplayPopupMenu(new Rect(mousePos.x, mousePos.y, 0, 0), MenuConfig.INSPECTOR, null);
                Event.current.Use();
            }

            return ForceFoldout ? true : foldoutRecords[Object][Path];
        }
        public static void DrawTab()
        {
            int width = (Path.Split('/').Length - 1) * 1;
            EditorGUILayout.LabelField("", GUILayout.Width(width));
        }
        public static bool DrawTabAndFoldout(GUIContent content, bool defaultFoldout = false)
        {
            var foldout = false;
            EditorGUILayout.BeginHorizontal();
            DrawTab();
            foldout = DrawFoldout(content, defaultFoldout);
            EditorGUILayout.EndHorizontal();
            return foldout;
        }

        public static bool CheckMember(MemberInfo info)
        {
            return MemberDrawer.Check(info);
        }
        public static bool DrawMember(MemberInfo info, GUIContent content = null)
        {
            return MemberDrawer.Draw(info, content);
        }

        public static bool DrawObject(object value, GUIContent content = null, bool defaultFoldout = false)
        {
            return ObjectDrawer.Draw(value, content, defaultFoldout);
        }

        public static bool DrawBool(ref bool value, GUIContent content)
        {
            var nextValue = EditorGUILayout.Toggle(content, value);
            var changed = value != nextValue;
            value = nextValue;
            return changed;
        }

        public static bool DrawInt(ref int value, GUIContent content)
        {
            var nextValue = EditorGUILayout.IntField(content, value);
            var changed = value != nextValue;
            value = nextValue;
            return changed;
        }

        public static bool DrawFloat(ref float value, GUIContent content)
        {
            var nextValue = EditorGUILayout.FloatField(content, value);
            var changed = value != nextValue;
            value = nextValue;
            return changed;
        }

        public static bool DrawEnum(ref Enum value, GUIContent content)
        {
            var nextValue = EditorGUILayout.EnumPopup(content, value);
            var changed = value.CompareTo(nextValue) != 0;
            value = nextValue;
            return changed;
        }

        public static bool DrawString(ref string value, GUIContent content)
        {
            var nextValue = EditorGUILayout.TextField(content, value);
            var changed = value != nextValue;
            value = nextValue;
            return changed;
        }

        public static bool DrawArray(object[] values, Type elementType, GUIContent content, bool isReadonly)
        {
            var preIsReadonly = ListDrawer.IsReadonly;
            ListDrawer.IsReadonly = isReadonly;

            var changed = ListDrawer.Draw(values.ToList(), elementType, content);

            ListDrawer.IsReadonly = preIsReadonly;

            return changed;
        }

        // Draw List<>, LinkedList<>
        public static bool DrawList(List<object> values, Type elementType, GUIContent content, bool isReadonly)
        {
            var preIsReadonly = ListDrawer.IsReadonly;
            ListDrawer.IsReadonly = isReadonly;

            var changed = ListDrawer.Draw(values, elementType, content);

            ListDrawer.IsReadonly = preIsReadonly;

            return changed;
        }

        public static bool DrawDict(Dictionary<object, object> values, Type keyType, Type valueType, GUIContent content, bool isReadonly)
        {
            var preIsReadonly = DictDrawer.IsReadonly;
            DictDrawer.IsReadonly = isReadonly;

            var changed = DictDrawer.Draw(values, keyType, valueType, content);

            DictDrawer.IsReadonly = preIsReadonly;

            return changed;
        }
    }
}