using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace UGFramework.Editor.Inspector
{
    public static class ListDrawer
    {
        public static bool IsReadonly { get; set; }

        static IList _iValues;
        static List<object> _values;

        public static bool Draw(List<object> values, IList iValues, GUIContent content)
        {
            _iValues = iValues;
            return Draw(values, content);
        }

        static bool Draw(List<object> values, GUIContent content)
        {
            _values = values;

            EditorGUILayout.BeginVertical();

            var listName = content.text;
            var prevPath = InspectorUtility.Path;
            InspectorUtility.AddFolder(listName);

            var changed = false; 
            EditorGUILayout.BeginHorizontal();
            var foldout = InspectorUtility.DrawTabAndFoldout(content);
            var added = false;
            var deleted = false;
            changed = DrawAddDelete(0, ref added, ref deleted, false);
            if (changed) InspectorUtility.Foldout(true);
            EditorGUILayout.EndHorizontal();

            if (changed == false && foldout)
            {
                var tmpValues = new List<object>(values);
                for (int i = 0; i < tmpValues.Count; ++i)
                {
                    changed |= DrawElement(tmpValues, i);
                }
            }

            InspectorUtility.Path = prevPath;
            EditorGUILayout.EndVertical();
            return changed;
        }

        static bool DrawElement(List<object> values, int index)
        {
            var prePath = InspectorUtility.Path;

            var name = "Element" + index;
            InspectorUtility.AddFolder(name);

            var info = new MemberInfo(values[index], name);
            EditorGUILayout.BeginHorizontal(); 
            InspectorUtility.DrawTab(); var changed = InspectorUtility.DrawMember(ref info);
            var added = false;
            var deleted = false;
            changed |= DrawAddDelete(index, ref added, ref deleted);
            EditorGUILayout.EndHorizontal();

            // Update value
            if (changed && deleted == false && added == false)
            {
                UpdateElement(index, info.Value);
            }

            InspectorUtility.Path = prePath;

            return changed;
        }

        static void UpdateElement(int index, object value)
        {
            _values[index] = value;
        }

        static bool DrawAddDelete(int index, ref bool added, ref bool deleted, bool isElement = true)
        {
            if (IsReadonly)
                return false;

            var changed = false;
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                changed = true;
                added = true;
                _AddElement(isElement == false ? -1 : index);
            }
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                changed = true;
                deleted = _DeleteElement(isElement == false ? 0 : index);
            }
            return changed;
        }

        static void _AddElement(int index)
        {
            var type = _iValues.GetType().GetGenericArguments()[0];
            object value = null;
            if (index != -1 || _values.Count != 0)
            {
                index = index == -1 ? _values.Count - 1 : index;
                var json = JsonConvert.SerializeObject(_values[index]);
                value = JsonConvert.DeserializeObject(json, type);
                _values.Insert(index + 1, value);
            }
            else
            {
                value = JsonConvert.DeserializeObject("", type);                
                _values.Insert(0, value);
            }
        }
        static bool _DeleteElement(int index)
        {
            if (_values.Count <= index)
                return false;

            _values.RemoveAt(index);
            return true;
        }
    }
}