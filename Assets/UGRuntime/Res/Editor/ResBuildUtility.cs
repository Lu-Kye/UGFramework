using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UGFramework.Log;
using UGFramework.Extension;

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

        public static void ResetBundleName(ref AssetBundleBuild buildInfo)
        {
            var assetBundleNameWithoutExtension = buildInfo.assetBundleName.ReplaceLast(ResConfig.BUNDLE_EXTENSION, "");
            assetBundleNameWithoutExtension = ResConfig.ConvertToBundleName(assetBundleNameWithoutExtension);
            var assetBundleName = assetBundleNameWithoutExtension + ResConfig.BUNDLE_EXTENSION;
            buildInfo.assetBundleName = assetBundleName;

            foreach (var assetName in buildInfo.assetNames)
            {
                var assetImporter = AssetImporter.GetAtPath(assetName);
                if (assetImporter == null)
                {
                    LogManager.Error("ResetBundleName error, asset not found: " + assetName);
                    continue;
                }
                if (assetImporter.assetBundleName != assetBundleName)
                {
                    assetImporter.assetBundleName = assetBundleName;
                }
            }
        }

        public static void AppendDependencies(AssetBundleBuild buildInfo, List<AssetBundleBuild> buildInfos)
        {
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
                // Exclude assets not in Assets
                if (dependence.StartsWith("Assets") == false)
                {
                    LogManager.Warning(string.Format(
                        "asset({0}) depends on asset({1}) not in Assets!",
                        name, 
                        dependence
                    ));
                    continue;
                }

                // Exclude scripts
                if (dependence.EndsWith(".cs") || dependence.EndsWith(".js"))
                {
                    continue;
                }

                if (dependence.StartsWith("Assets/" + ResConfig.RES_ROOT))
                {
                    if (name != dependence && 
                        dependence.EndsWith(".shader") == false)
                    {
                        LogManager.Warning(string.Format(
                            "asset({0}) depends on asset({1}) in Assets/Res, which should be prevented!",
                            name, 
                            dependence
                        ));
                    }
                    continue;
                }

                // Set atlas name 
                if (dependence.StartsWith("Assets/" + ResConfig.UI_TEXTURE) &&
                    dependence.StartsWith("Assets/" + ResConfig.UI_TEXTURE_RUNTIME) == false)
                {
                    var textureImporter = AssetImporter.GetAtPath(dependence) as TextureImporter;
                    ResTextureAutoConfigurer.ConfigureUITexture(textureImporter);
                }

                var assetName = dependence;
                var assetBundleName = dependence;
                var dependenceRoot = dependence.ReplaceFirst("Assets", ResConfig.DEPENDENCIES_ROOT);
                if (ResConfig.IsFolderAsBundleName(dependence))
                {
                    var fileName = Path.GetFileName(dependence);
                    assetBundleName = dependenceRoot.ReplaceLast("/" + fileName, "") + ResConfig.BUNDLE_EXTENSION;
                }
                else
                {
                    assetBundleName = dependenceRoot + ResConfig.BUNDLE_EXTENSION;
                }

                var index = TryGetBuildInfo(buildInfos, assetBundleName);
                if (index == -1)
                {
                    var dependenceBuildInfo = new AssetBundleBuild();
                    dependenceBuildInfo.assetNames = new string[] { assetName };
                    dependenceBuildInfo.assetBundleName = assetBundleName;
                    buildInfos.Add(dependenceBuildInfo);
                    _AppendDependencies(dependenceBuildInfo, assetName, buildInfos);
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
                    _AppendDependencies(dependenceBuildInfo, assetName, buildInfos);
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

        public static void ClearAssetBundleConfigurations()
        {
            EditorUtility.DisplayProgressBar(
                "Clear AssetBundle Configurations", 
                string.Format("Clearing bundleNames..."),
                (float)1 / 1);
            var names = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < names.Length; ++i)
            {
                AssetDatabase.RemoveAssetBundleName(names[i], true);
            }
            EditorUtility.ClearProgressBar();
        }
    }
}