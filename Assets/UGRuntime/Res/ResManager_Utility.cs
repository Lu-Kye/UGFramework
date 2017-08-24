using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UGFramework.Res
{
    public partial class ResManager
    {
        ResVersionFile GetLocalVersionFile()
        {
            ResVersionFile versionFile;
            var txtAsset = this.Load<TextAsset>(ResConfig.VERSION_FILE);
            var txt = txtAsset.text;
            if (string.IsNullOrEmpty(txt) == false)
                versionFile = ResVersionFile.UnSerialize(txt);
            else
                versionFile = new ResVersionFile(0);
            
            // Unload
            this.UnloadAsset(ResConfig.VERSION_FILE);
    
            return versionFile;
        }
    
        string[] GetDependencies(string bundleName, string path)
        {
            if (_dependenceInfos.ContainsKey(bundleName))
            {
                return _dependenceInfos[bundleName];
            }
    
            string[] dependencies;
    
    #if UNITY_EDITOR
            if (this.Simulate == false)
            {
                dependencies = UnityEditor.AssetDatabase.GetAssetBundleDependencies(bundleName.ToLower(), true);
                _dependenceInfos[bundleName] = dependencies;
                return dependencies;
            }
    #endif
    
            if (_manifest == null)
            {
                return new string[0];
            }
            else
            {
                dependencies = _manifest.GetAllDependencies(bundleName);
            }
            _dependenceInfos[bundleName] = dependencies;
            return dependencies;
        }
    }
}