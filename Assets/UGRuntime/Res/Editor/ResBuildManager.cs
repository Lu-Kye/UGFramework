using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UGFramework.Editor;
using UGFramework.Log;
using UGFramework.Utility;
using UnityEditor;
using UnityEngine;

namespace UGFramework.Res
{
    public static class ResBuildManager
    {
        // Save builded resources to out path
        static readonly string _outPath = Application.streamingAssetsPath + "/" + ResConfig.RES_ROOT.ToLower();

        public static void Clear()
        {
            if (Directory.Exists(_outPath))
                Directory.Delete(_outPath, true);
            AssetDatabase.Refresh();
        }

        static void BeforeBuild(BuildTarget targetPlatform)
        {
            if (Directory.Exists(_outPath) == false)
                Directory.CreateDirectory(_outPath);
            PlatformUtility.Switch(targetPlatform);
        }

        public static void Build(BuildTarget targetPlatform)
        {
            BeforeBuild(targetPlatform);

            // Build bundles first
            var resPath = Application.dataPath + "/" + ResConfig.RES_ROOT;
            var files = ResBuildUtility.GetFiles(resPath);
            BuildBundles(targetPlatform, files);

            // Build version file
            var bundlesPath = _outPath;
            files = ResBuildUtility.GetFiles(bundlesPath);
            BuildVersionFile(targetPlatform, files);
        }

        static void BuildBundles(BuildTarget targetPlatform, string[] files)
        {
            var buildInfos = new Dictionary<string, AssetBundleBuild>();

            var filters = new List<ResAbstractBuildFilter>() {
                new ResPrefabBuildFilter(),
                new ResLuaBuildFilter(),
            };

            // BeforeBuild
            foreach (var filter in filters)
            {
                filter.BeforeBuild();
            }

            // Prepare build
            foreach (var file in files)
            {
                foreach (var filter in filters)
                {
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
            AssetDatabase.Refresh();

            try
            {
                var filteredBuildInfos = new List<AssetBundleBuild>();

                // Real build
                var overrideFilters = new HashSet<ResAbstractBuildFilter>();
                foreach (var pair in buildInfos)
                {
                    var filepath = pair.Key;
                    var buildInfo = pair.Value;

                    var overrided = false;
                    foreach (var filter in filters)
                    {
                        overrided |= filter.OverrideBuild(filepath, buildInfo);
                        if (overrided)
                        {
                            overrideFilters.Add(filter);
                            if (filter.BlockOthers)
                                break;
                        }
                    }

                    if (overrided == false)
                    {
                        // Add dependent buildInfos
                        ResBuildUtility.AppendDependencies(buildInfo, filteredBuildInfos);

                        // Add self buildInfo
                        filteredBuildInfos.Add(buildInfo);
                    }
                }

                // Build override filters first
                foreach (var filter in overrideFilters)
                {
                    filter.DoOverrideBuild(_outPath, filteredBuildInfos);
                }

                // Build 
                var options = BuildAssetBundleOptions.DeterministicAssetBundle;
                BuildPipeline.BuildAssetBundles(
                    _outPath, 
                    filteredBuildInfos.ToArray(),
                    options,
                    targetPlatform
            );
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
                AssetDatabase.RemoveUnusedAssetBundleNames();
                AssetDatabase.Refresh();
            }
        }

        static void BuildVersionFile(BuildTarget targetPlatform, string[] files)
        {
            var versionFile = new ResVersionFile(files.Length);
            for (var i = 0; i < files.Length; ++i)
            {
                var file = files[i];
                var versionInfo = new ResVersionInfo();
                versionInfo.MD5 = MD5Utility.GetFileMD5(file);

                versionFile.Files[i] = file.ReplaceFirst(_outPath + "/", "");
                versionFile.Infos[i] = versionInfo;
            }
            FileUtility.WriteFile(_outPath + "/" + ResConfig.VERSION_FILE, versionFile.Serialize());
            AssetDatabase.Refresh();
        }
    }
}
