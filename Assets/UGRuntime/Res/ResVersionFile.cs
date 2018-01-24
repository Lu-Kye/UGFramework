using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UGFramework.Res
{
    [Serializable]
    public struct ResVersionInfo
    {
        public string File;
        public string MD5;
        public ulong Size;
    }
    
    [Serializable]
    public struct ResVersionFile
    {
        public string Version;
        public int FileCount;
        public string[] Files;
        public ResVersionInfo[] Infos;
    
        public ResVersionFile(int length)
        {
            this.Version = ResConfig.VERSION;
            this.FileCount = length;
            this.Files = new string[length];
            this.Infos = new ResVersionInfo[length];
        }
    
        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }
    
        public static ResVersionFile UnSerialize(string json) 
        {
            return JsonUtility.FromJson<ResVersionFile>(json);
        }

        public static bool DebugDiffInfos = false;
        public static List<ResVersionInfo> GetDiffInfos(ResVersionFile local, ResVersionFile remote)
        {
            var diffInfos = new List<ResVersionInfo>();

            var localFiles = local.Files.ToList();
            // Compare
            for (int i = 0; i < remote.Files.Length; ++i)
            {
                var remoteFile = remote.Files[i];
                var remoteInfo = remote.Infos[i];
                remoteInfo.File = remoteFile;

                // Debug ?
                if (DebugDiffInfos)
                {
                    diffInfos.Add(remoteInfo);
                    continue;
                }
    
                // Is exists ?
                var localIndex = localFiles.IndexOf(remoteFile);
                if (localIndex >= 0 == false)
                {
                    diffInfos.Add(remoteInfo);
                    continue;
                }
    
                // Is same md5 ?
                var localInfo = local.Infos[localIndex];
                if (localInfo.MD5 != remoteInfo.MD5)
                    diffInfos.Add(remoteInfo);
            }
    
            return diffInfos;
        }
    }
}