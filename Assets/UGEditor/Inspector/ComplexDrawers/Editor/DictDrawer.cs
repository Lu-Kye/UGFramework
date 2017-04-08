using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace UGFramework.Editor.Inspector
{
    public static class DictDrawer
    {
        public static bool IsReadonly { get; set; }

        static Type _keyType;
        static Type _valueType;

        static List<MemberInfo.DictElement> _values;

        public static bool Draw(List<MemberInfo.DictElement> values, Type keyType, Type valueType, GUIContent content)
        {
            _keyType = keyType;
            _valueType = valueType;
            return Draw(values, content);
        }

        static bool Draw(List<MemberInfo.DictElement> values, GUIContent content)
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
                var tmpValues = new List<MemberInfo.DictElement>(values);
                for (int i = 0; i < tmpValues.Count; ++i)
                {
                    changed |= DrawElement(tmpValues, i);
                }
            }

            InspectorUtility.Path = prevPath;
            EditorGUILayout.EndVertical();
            return changed;
        }

        static bool DrawElement(List<MemberInfo.DictElement> values, int index)
        {
            var prePath = InspectorUtility.Path;

            var name = "Element" + index + "    Key: " + values[index].Key.ToString();
            var info = new MemberInfo(values[index], name);
            EditorGUILayout.BeginHorizontal(); 
            InspectorUtility.DrawTab(); 
            var changed = InspectorUtility.DrawMember(info);
            var added = false;
            var deleted = false;
            changed |= DrawAddDelete(index, ref added, ref deleted);
            EditorGUILayout.EndHorizontal();

            // Update value
            if (changed && deleted == false && added == false)
            {
                UpdateElement(index, info.Value as MemberInfo.DictElement);
            }

            InspectorUtility.Path = prePath;

            return changed;
        }

        static void UpdateElement(int index, MemberInfo.DictElement value)
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
            object key = null;
            object value = null;
            if (index != -1 || _values.Count != 0)
            {
                index = index == -1 ? _values.Count - 1 : index;
                var json = JsonConvert.SerializeObject(_values[index].Key);
                key = JsonConvert.DeserializeObject(json, _keyType);

                json = JsonConvert.SerializeObject(_values[index].Value);
                value = JsonConvert.DeserializeObject(json, _valueType);

                var element = new MemberInfo.DictElement();
                element.Key = key;
                element.Value = value;
                _values.Insert(index + 1, element);
            }
            else
            {
                if (_keyType.IsValueType && _keyType.IsPrimitive)
                    key = Activator.CreateInstance(_keyType);
                else
                    key = JsonConvert.DeserializeObject("{}", _keyType);                

                if (_valueType.IsValueType && _valueType.IsPrimitive)
                    value = Activator.CreateInstance(_valueType);
                else
                    value = JsonConvert.DeserializeObject("{}", _valueType);                

                var element = new MemberInfo.DictElement();
                element.Key = key;
                element.Value = value;
                _values.Insert(0, element);
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