using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UGFramework.Log;
using UGFramework.Extension.String;

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
            for (int i = 0; i < buildInfo.assetNames.Length; ++i)
            {
                _AppendDependencies(buildInfo, buildInfo.assetNames[i], buildInfos);
            }
        }
        static void _AppendDependencies(AssetBundleBuild buildInfo, string name, List<AssetBundleBuild> buildInfos)
        {
            var dependencies = AssetDatabase.GetDependencies(name, true);
            foreach (var dependence in dependencies)
            {
                if (dependence.StartsWith("Assets/" + ResConfig.RES_ROOT))
                    continue;

                // Exclude code files
                if (dependence.EndsWith(".cs") || dependence.EndsWith(".js"))
                    continue;

                var assetName = dependence;
                var assetBundleName = dependence;
                if (dependence.StartsWith("Assets/" + ResConfig.UI_TEXTURE))
                {
                    if (dependence.StartsWith("Assets/" + ResConfig.UI_TEXTURE_RUNTIME))
                        continue;

                    var fileName = Path.GetFileName(dependence);
                    assetBundleName = dependence.Replace("Assets", ResConfig.DEPENDENCIES_ROOT).ReplaceLast("/" + fileName, "") + ResConfig.BUNDLE_EXTENSION;
                }
                else
                {
                    assetBundleName = dependence.ReplaceFirst("Assets", ResConfig.DEPENDENCIES_ROOT) + ResConfig.BUNDLE_EXTENSION;
                }

                var index = TryGetBuildInfo(buildInfos, assetBundleName);
                if (index == -1)
                {
                    var dependenceBuildInfo = new AssetBundleBuild();
                    dependenceBuildInfo.assetNames = new string[] { assetName };
                    dependenceBuildInfo.assetBundleName = assetBundleName;
                    buildInfos.Add(dependenceBuildInfo);

                    LogManager.Log("dependence " + assetBundleName);
                }
                else
                {
                    var dependenceBuildInfo = buildInfos[index];
                    var assetNames = dependenceBuildInfo.assetNames.ToList();
                    if (assetNames.Contains(assetName))
                        continue;
                    
                    assetNames.Add(assetName);
                    dependenceBuildInfo.assetNames = assetNames.ToArray();
                    buildInfos[index] = dependenceBuildInfo;

                    LogManager.Log("dependence " + assetBundleName);
                }
            }
        }

        public static int TryGetBuildInfo(List<AssetBundleBuild> sourceBuildInfos, string assetBundleName)
        {
            var index = 0;
            foreach (var sourceBuildInfo in sourceBuildInfos) 
            {
                if (sourceBuildInfo.assetBundleName == assetBundleName)
                    return index;
                index++;
            }
            return -1;
        }
    }
}