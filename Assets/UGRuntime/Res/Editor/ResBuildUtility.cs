using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UGFramework.Log;

namespace UGFramework.Res
{
    public static class ResBuildUtility
    {
        public static string[] GetFiles(string path)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            var assetFiles = new List<string>();
            for (var i = 0; i < files.Length; ++i)
            {
                var file = files[i].Replace('\\', '/');
                if (file.EndsWith(".meta") ||
                    file.EndsWith(".keep") ||
                    file.EndsWith(".DS_Store"))
                    continue;

                assetFiles.Add(file);
            }
            return assetFiles.ToArray();
        }

        public static void SetBundleName(AssetBundleBuild buildInfo)
        {
            var assetImporter = AssetImporter.GetAtPath(buildInfo.assetNames[0]);
            assetImporter.assetBundleName = buildInfo.assetBundleName;
        }

        public static void AppendDependencies(AssetBundleBuild buildInfo, List<AssetBundleBuild> buildInfos)
        {
            var dependencies = AssetDatabase.GetDependencies(buildInfo.assetNames[0], true);
            foreach (var dependence in dependencies)
            {
                if (dependence.StartsWith("Assets/" + ResConfig.RES_ROOT))
                    continue;

                var assetName = dependence;
                var assetBundleName = Path.GetDirectoryName(dependence).ReplaceFirst("Assets", ResConfig.DEPENDENCIES_ROOT).ToLower() + ResConfig.BUNDLE_EXTENSION;

                AssetBundleBuild dependenceBuildInfo;
                var dependenceBuildInfoNullable = TryGetBuildInfo(buildInfos, assetBundleName);
                if (dependenceBuildInfoNullable == null)
                {
                    dependenceBuildInfo = new AssetBundleBuild();
                    dependenceBuildInfo.assetNames = new string[] { assetName };
                    dependenceBuildInfo.assetBundleName = assetBundleName;
                    buildInfos.Add(dependenceBuildInfo);

                    LogManager.Log("dependence " + assetBundleName);
                }
                else
                {
                    dependenceBuildInfo = (AssetBundleBuild)dependenceBuildInfoNullable;
                    var assetNames = dependenceBuildInfo.assetNames.ToList();
                    if (assetNames.Contains(assetName))
                        continue;
                    
                    assetNames.Add(assetName);
                    dependenceBuildInfo.assetNames = assetNames.ToArray();

                    LogManager.Log("dependence " + assetBundleName);
                }
                SetBundleName(dependenceBuildInfo);
            }
        }

        static AssetBundleBuild? TryGetBuildInfo(List<AssetBundleBuild> sourceBuildInfos, string assetBundleName)
        {
            foreach (var sourceBuildInfo in sourceBuildInfos) 
            {
                if (sourceBuildInfo.assetBundleName == assetBundleName)
                    return sourceBuildInfo;
            }
            return null;
        }
    }
}