using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UGFramework.Editor.Inspector
{
    /**
     * --- DOC BEGIN ---
     * As default, only draw public fields
     * --- DOC END ---
     */
    public static class ObjectDrawer
    {
        public static bool Draw(object obj, GUIContent content = null)
        {
            if (obj == null)
            {
                if (content == null)
                    return false;
                EditorGUILayout.LabelField(content, new GUIContent("null"));
                return false;
            }

            var type = obj.GetType();

            var memberInfos = new List<MemberInfo>();
            // Get all public or accessible fields
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; ++i)
            {
                var field = fields[i];
                var accessible = field.GetCustomAttributes(typeof(ShowInInspector), false).Length > 0;
                if (field.IsPublic || accessible) 
                {
                    var memberInfo = new MemberInfo(obj, field); 
                    if (InspectorUtility.CheckMember(memberInfo) == false)
                        continue;
                    memberInfos.Add(memberInfo);
                }
            }
            // Get all accessible properties
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < properties.Length; ++i)
            {
                var property = properties[i];
                if (property.GetGetMethod(true) == null)
                {
                    continue;
                }

                var accessible = property.GetCustomAttributes(typeof(ShowInInspector), false).Length > 0;
                if (accessible)
                {
                    var memberInfo = new MemberInfo(obj, property); 
                    if (InspectorUtility.CheckMember(memberInfo) == false)
                        continue;
                    memberInfos.Add(memberInfo);
                }
            }

            // Draw
            EditorGUILayout.BeginVertical();
            var foldout = false;
            // Show foldout when content is not null
            if (content != null)
            {
                EditorGUILayout.BeginHorizontal();
                InspectorUtility.DrawTab();
                foldout = InspectorUtility.Foldout(content);
                EditorGUILayout.EndHorizontal();
                InspectorUtility.AddFolder(content.text);
            }
            else
            {
                foldout = true;
            }
            if (foldout == false)
            {
                EditorGUILayout.EndVertical();
                return false;
            }

            var changed = false;
            var iter = memberInfos.GetEnumerator();
            while (iter.MoveNext())
            {
                var prePath = InspectorUtility.Path;
                EditorGUILayout.BeginHorizontal();
                InspectorUtility.DrawTab();
                changed |= InspectorUtility.DrawMember(iter.Current); 
                EditorGUILayout.EndHorizontal();
                InspectorUtility.Path = prePath;
            }
            
            EditorGUILayout.EndVertical();

            return changed;
        }
    }
}