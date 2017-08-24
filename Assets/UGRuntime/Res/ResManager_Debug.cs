using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UGFramework.UGEditor.Inspector;
#endif

namespace UGFramework.Res
{
    public partial class ResManager 
    {
#if UNITY_EDITOR
        [ShowInInspector(IsReadonly = true)]
        [OverrideDrawer("OnDrawLoadedAssetBundles")]
#endif
        List<string> _loadedAssetBundles = new List<string>();

        void DebugAllocBundle(string bundleName)
        {
#if UNITY_EDITOR
            if (_loadedAssetBundles.Contains(bundleName) == false)
                _loadedAssetBundles.Add(bundleName);    
#endif
        }

        void DebugDeallocBundle(string bundleName)
        {
#if UNITY_EDITOR
            _loadedAssetBundles.Remove(bundleName);    
#endif
        }

        bool _foldout = true;
        string _search = "";
        public bool OnDrawLoadedAssetBundles(GUIContent content)
        {
#if UNITY_EDITOR
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            _foldout = EditorGUILayout.Foldout(_foldout, content, true);
            EditorGUILayout.EndHorizontal();

            if (_foldout)
            {
                var loadedAssetBundleNames = _loadedAssetBundles;

                // Count
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(10));
                EditorGUILayout.LabelField("Count: " + _loadedAssetBundles.Count);
                EditorGUILayout.EndHorizontal();

                // Search
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(10));
                _search = EditorGUILayout.TextField("Search: ", _search);
                EditorGUILayout.EndHorizontal();

                foreach (var loadedAssetBundleName in loadedAssetBundleNames)
                {
                    if (string.IsNullOrEmpty(_search) == false)
                    {
                        if (loadedAssetBundleName.Contains(_search) == false)
                            continue;
                    }

                    var loadedAssetBundle = _assetBundles[loadedAssetBundleName];
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("", GUILayout.Width(10));
                    EditorGUILayout.LabelField(loadedAssetBundle.ReferencedCount.ToString(), GUILayout.Width(20));
                    EditorGUILayout.LabelField(loadedAssetBundleName);

                    EditorGUILayout.EndHorizontal();
                }

            }

            EditorGUILayout.EndVertical();
            return true;
#else
            return false;
#endif
        }
    }
}