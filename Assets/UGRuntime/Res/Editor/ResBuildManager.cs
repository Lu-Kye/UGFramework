using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UGFramework.UGEditor;
using UGFramework.Log;
using UGFramework.Utility;
using UGFramework.Extension;
using UnityEditor;
using UnityEngine;

namespace UGFramework.Res
{
    public static class ResBuildManager
    {
        static BuildTarget _target;

        // Save builded resources to out path
        static string _outPath 
        {
            get
            {
                string platformPrefixPath = null;
                if (_target == BuildTarget.iOS)
                {
                    platformPrefixPath = ResConfig.PLATFORM_PREFIX_IOS;
                }
                else if (_target == BuildTarget.Android)
                {
                    platformPrefixPath = ResConfig.PLATFORM_PREFIX_ANDROID;
                }
                else if (_target == BuildTarget.StandaloneWindows64)
                {
                    platformPrefixPath = ResConfig.PLATFORM_PREFIX_WIN;
                }
                else if (_target == BuildTarget.StandaloneOSX)
                {
                    platformPrefixPath = ResConfig.PLATFORM_PREFIX_OSX;
                }
                return Application.streamingAssetsPath + "/" + platformPrefixPath + "/" + ResConfig.RES_ROOT.ToLower();
            }
        }

        static BuildAssetBundleOptions _options = 
                    // BuildAssetBundleOptions.DisableWriteTypeTree |
                    // BuildAssetBundleOptions.DeterministicAssetBundle |
                    // BuildAssetBundleOptions.StrictMode |
                    // BuildAssetBundleOptions.ForceRebuildAssetBundle |
                    BuildAssetBundleOptions.ChunkBasedCompression;
                    // BuildAssetBundleOptions.UncompressedAssetBundle;

        public static void Clear()
        {
            EditorUtility.DisplayProgressBar(
                "Clear AssetBundles", 
                string.Format("Removing..."),
                1f);
            if (Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.Delete(Application.streamingAssetsPath, true);
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            EditorUtility.ClearProgressBar();

            ResBuildUtility.ClearAssetBundleConfigurations();
            AssetDatabase.Refresh();
        }

        static void BeforeBuild(BuildTarget targetPlatform)
        {
            EditorUtility.DisplayProgressBar(
                "Building AssetBundles", 
                string.Format("Starting building..."),
                0);

            _target = targetPlatform;

            if (Directory.Exists(_outPath) == false)
                Directory.CreateDirectory(_outPath);
            ResBuildUtility.ClearAssetBundleConfigurations();
        }

        static void AfterBuild()
        {
            EditorUtility.ClearProgressBar();
        }

        public static void Build(BuildTarget targetPlatform)
        {
            BeforeBuild(targetPlatform);

            // Build 
            var resPath = Application.dataPath + "/" + ResConfig.RES_ROOT;
            var files = ResBuildUtility.GetFiles(resPath);
            BuildBundles(targetPlatform, files);

            AfterBuild();
        }

        static void BuildBundles(BuildTarget targetPlatform, string[] files)
        {
            var buildInfos = new Dictionary<string, AssetBundleBuild>();

            var filters = new List<ResAbstractBuildFilter>() {
                new ResLuaBuildFilter(),
                new ResSpriteBuildFilter(),

                // Assets contain dependence
                new ResPrefabBuildFilter(),
            };

            // 10
            var basePercent = 0f;
            var percent = 10f;
            var index = 0f;
            var count = filters.Count;

            // BeforeBuild
            EditorUtility.DisplayProgressBar(
                "Building AssetBundles", 
                string.Format("Before building..."),
                ((1) * percent + basePercent) / 100);
            foreach (var filter in filters)
            {
                filter.BeforeBuild();
                index++;
            }

            // 30
            basePercent = basePercent + percent;
            percent = 20f;
            index = 0;
            count = files.Length * filters.Count; 

            // Prepare build
            EditorUtility.DisplayProgressBar(
                "Building AssetBundles", 
                string.Format("Preparing..."),
                ((1) * percent + basePercent) / 100);
            foreach (var file in files)
            {
                foreach (var filter in filters)
                {
                    index++;

                    if (filter.Filtered(file))
                    {
                        AssetBundleBuild? buildInfoNullable = filter.PrepareBuild(file);
                        if (buildInfoNullable != null)
                        {
                            var buildInfo = (AssetBundleBuild)buildInfoNullable;
                            buildInfos[file] = buildInfo;
                        }

                        if (filter.BlockOthers)
                            break;
                    }
                }
            }

            // 50
            basePercent = basePercent + percent;
            percent = 20f;
            index = 0;
            count = files.Length * buildInfos.Count; 

            try
            {
                var filteredBuildInfos = new List<AssetBundleBuild>();

                // Real build
                foreach (var pair in buildInfos)
                {
                    var filepath = pair.Key;
                    var buildInfo = pair.Value;
                    {
                        // Add dependent buildInfos
                        ResBuildUtility.AppendDependencies(buildInfo, filteredBuildInfos);

                        // Add self buildInfo
                        filteredBuildInfos.Add(buildInfo);
                    }
                }

                // 60
                basePercent = basePercent + percent;
                percent = 10f;
                index = 0;
                count = filters.Count;

                // Additive build
                foreach (var filter in filters)
                {
                    EditorUtility.DisplayProgressBar(
                        "Building AssetBundles", 
                        string.Format(filter.GetType().Name + " override building..."),
                        ((index / count) * percent + basePercent) / 100);
                    index++;

                    filter.OverrideBuild(_outPath, filteredBuildInfos);
                }

                // 80
                basePercent = basePercent + percent;
                percent = 20f;
                index = 0;
                count = filteredBuildInfos.Count;

                // Set bundle name
                EditorUtility.DisplayProgressBar(
                    "Building AssetBundles", 
                    string.Format("Setting bundleNames..."),
                    ((1) * percent + basePercent) / 100);
                for (var i = 0; i < filteredBuildInfos.Count; ++i)
                {
                    index++;

                    var buildInfo = filteredBuildInfos[i];
                    ResBuildUtility.ResetBundleName(ref buildInfo);
                    filteredBuildInfos[i] = buildInfo;
                }

                // Build 
                BuildPipeline.BuildAssetBundles(
                    _outPath, 
                    // filteredBuildInfos.ToArray(),
                    _options,
                    targetPlatform
                );

                // 100
                basePercent = basePercent + percent;
                percent = 20f;
                index = 1;
                count = 1;

                // Build version file
                EditorUtility.DisplayProgressBar(
                    "Building AssetBundles", 
                    string.Format("Building version file..."),
                    ((index / count) * percent + basePercent) / 100);

                var bundlesPath = _outPath;
                files = ResBuildUtility.GetFiles(bundlesPath);
                BuildVersionFile(files);
            }
            catch (Exception e)
            {
                LogManager.Error(e.Message);
            }
            finally
            {
                // After build
                foreach (var filter in filters)
                {
                    filter.AfterBuild();
                }
            }
        }

        static ResVersionFile BuildVersionFile(string[] files)
        {
            var versionFile = new ResVersionFile(files.Length);
            for (var i = 0; i < files.Length; ++i)
            {
                var file = files[i];
                var versionInfo = new ResVersionInfo();
                versionInfo.File = file.ReplaceFirst(_outPath + "/", "");
                versionInfo.MD5 = MD5Utility.GetFileMD5(file);
                versionInfo.Size = (ulong)FileUtility.ReadFileBytes(file).Length;

                versionFile.Infos[i] = versionInfo;
            }

            var filePath = Application.dataPath + "/" + ResConfig.RES_ROOT + "/" + ResConfig.VERSION_FILE;
            FileUtility.WriteFile(filePath, versionFile.Serialize());
            AssetDatabase.Refresh();

            var buildInfo = new AssetBundleBuild();
            buildInfo.assetBundleName = ResConfig.VERSION_FILE;
            var assetName = "Assets/" + ResConfig.RES_ROOT + "/" + ResConfig.VERSION_FILE;
            buildInfo.assetNames = new string[] { assetName };
            ResBuildUtility.ResetBundleName(ref buildInfo);

            var options = _options & (~BuildAssetBundleOptions.ForceRebuildAssetBundle);
            BuildPipeline.BuildAssetBundles(
                _outPath, 
                // new AssetBundleBuild[] { buildInfo },
                options,
                _target
            );

            // Copy & Remove temp versionFile
            FileUtility.CopyFile(filePath, _outPath + "/" + ResConfig.VERSION_FILE);
            FileUtility.DeleteFile(filePath);

            return versionFile;
        }
    }
}
