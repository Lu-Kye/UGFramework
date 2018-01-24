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
        public ResVersionInfo[] Infos;
        public ResVersionInfo? Get(string file)
        {
            foreach (var info in this.Infos)
            {
                if (info.File == file)
                    return info;
            }
            return null;
        }
    
        public ResVersionFile(int length)
        {
            this.Version = ResConfig.VERSION;
            this.FileCount = length;
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

            // Compare
            for (int i = 0; i < remote.Infos.Length; ++i)
            {
                var remoteInfo = remote.Infos[i];
                var localInfo = local.Get(remoteInfo.File);

                // Debug ?
                if (DebugDiffInfos)
                {
                    diffInfos.Add(remoteInfo);
                    continue;
                }

                // Is exists ?
                if (localInfo == null)
                {
                    diffInfos.Add(remoteInfo);
                    continue;
                }

                // Is same md5 ?
                if (((ResVersionInfo)localInfo).MD5 != remoteInfo.MD5)
                    diffInfos.Add(remoteInfo);
            }

            return diffInfos;
        }
    }
}