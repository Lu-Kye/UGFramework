using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

namespace UGFramework.UGEditor.Inspector
{
    [Serializable]
    public class DictElement<TK, TV>
    {
        public TK Key;
        public TV Value;
    }

    public static class DictHelper
    {
        public static Dictionary<object, object> Get(IDictionary iValues)
        {
            var values = new Dictionary<object, object>();
            foreach (var key in iValues.Keys)
            {
                values[key] = iValues[key];    
            }
            return values;
        }

        public static List<object> Get(Dictionary<object, object> dict, Type keyType, Type valueType)
        {
            var values = new List<object>();
            foreach (var pair in dict)
            {
                var element = CreateElement(keyType, valueType);
                SetKey(element, pair.Key);
                SetValue(element, pair.Value);
                values.Add(element);
            }
            return values;
        }

        public static void Set(Dictionary<object, object> dict, List<object> values)
        {
            dict.Clear();
            foreach (var element in values)
            {
                dict[GetKey(element)] = GetValue(element);
            }
        }

        public static object CreateElement(Type keyType, Type valueType)
        {
            var type = typeof(DictElement<,>);
            Type[] typeArgs = { keyType, valueType };
            var genericType = type.MakeGenericType(typeArgs);
            return Activator.CreateInstance(genericType);
        }

        public static object GetKey(object element)
        {
            return element.GetType().GetField("Key").GetValue(element);
        }
        public static void SetKey(object element, object key)
        {
            element.GetType().GetField("Key").SetValue(element, key);
        }

        public static object GetValue(object element)
        {
            return element.GetType().GetField("Value").GetValue(element);
        }
        public static void SetValue(object element, object value)
        {
            element.GetType().GetField("Value").SetValue(element, value);
        }
    }

    public static class DictDrawer
    {
        public static bool IsReadonly { get; set; }

        static Type _keyType;
        static Type _valueType;

        static List<object> _values;

        public static bool Draw(Dictionary<object, object> values, Type keyType, Type valueType, GUIContent content)
        {
            _values = DictHelper.Get(values, keyType, valueType);
            _keyType = keyType;
            _valueType = valueType;

            var changed = Draw(_values, content);
            if (changed)
                DictHelper.Set(values, _values);
            
            return changed;
        }

        static bool Draw(List<object> values, GUIContent content)
        {
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

            var element = values[index];
            var key = element.GetType().GetField("Key").GetValue(element);
            var name = "Element" + index + "    Key: " + key.ToString();
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
                UpdateElement(index, info.Value as object);
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
            object key = null;
            object value = null;
            if (index != -1 || _values.Count != 0)
            {
                index = index == -1 ? _values.Count - 1 : index;

                var preElement = _values[index];

                string json;
                if (_keyType == typeof(int) ||
                    _keyType == typeof(uint) ||
                    _keyType == typeof(double) ||
                    _keyType == typeof(float) 
                )
                {
                    key = -9999;
                }
                else if (_keyType == typeof(string))
                {
                    key = "Null";
                }
                else
                {
                    json = JsonConvert.SerializeObject(preElement.GetType().GetField("Key").GetValue(preElement));
                    key = JsonConvert.DeserializeObject(json, _keyType);
                }

                json = JsonConvert.SerializeObject(preElement.GetType().GetField("Value").GetValue(preElement));
                value = JsonConvert.DeserializeObject(json, _valueType);

                var element = DictHelper.CreateElement(_keyType, _valueType);
                DictHelper.SetKey(element, key);
                DictHelper.SetValue(element, value);
                _values.Insert(index + 1, element);
            }
            else
            {
                if (_keyType == typeof(int) ||
                    _keyType == typeof(uint) ||
                    _keyType == typeof(double) ||
                    _keyType == typeof(float) 
                )
                    key = -9999;
                else if (_keyType == typeof(string))
                    key = "Null";
                else
                    key = JsonConvert.DeserializeObject("{}", _keyType);                

                if (_valueType.IsValueType && _valueType.IsPrimitive)
                    value = Activator.CreateInstance(_valueType);
                else
                    value = JsonConvert.DeserializeObject("{}", _valueType);                

                var element = DictHelper.CreateElement(_keyType, _valueType);
                DictHelper.SetKey(element, key);
                DictHelper.SetValue(element, value);
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