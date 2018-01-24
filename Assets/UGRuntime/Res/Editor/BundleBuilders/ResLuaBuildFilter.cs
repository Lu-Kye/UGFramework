using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UGFramework.Extension;
using UGFramework.Log;
using UGFramework.Utility;

namespace UGFramework.Res
{
    public class ResLuaBuildFilter : ResAbstractBuildFilter
    {
        public override string FileExtension { get { return "unknown"; } }
        public ResLuaBuildFilter() : base() {}

        List<string> _tmpFiles = new List<string>();

        public override void OverrideBuild(string outpathRoot, List<AssetBundleBuild> filteredBuildInfos)
        {
            for (int i = filteredBuildInfos.Count - 1; i >= 0; --i)
            {
                var buildInfo = filteredBuildInfos[i];
                if (buildInfo.assetBundleName.StartsWith("Assets/" + ResConfig.RES_ROOT + "/" + ResConfig.LUA_ROOT) ||
                    buildInfo.assetBundleName.StartsWith(ResConfig.DEPENDENCIES_ROOT + "/" + ResConfig.RES_ROOT + "/" + ResConfig.LUA_ROOT))
                {
                    filteredBuildInfos.RemoveAt(i);
                }
            }

            var resPath = Application.dataPath + "/" + ResConfig.RES_ROOT + "/" + ResConfig.LUA_ROOT;
            var files = ResBuildUtility.GetFiles(resPath);
            _tmpFiles.Clear();

            foreach (var file in files)
            {
                if (file.EndsWith(ResConfig.LUA_EXTENSION) == false)
                    continue;

                var sourceFile = file;
                var targetFile = sourceFile.ReplaceLast(ResConfig.LUA_EXTENSION, ResConfig.MOBILE_LUA_EXTENSION);

                FileUtility.MoveFile(sourceFile, targetFile);
                FileUtility.MoveFile(sourceFile+".meta",targetFile+".meta");
                _tmpFiles.Add(targetFile);

                var assetBundleName = targetFile.ReplaceFirst(Application.dataPath + "/" + ResConfig.RES_ROOT + "/", "") + ResConfig.BUNDLE_EXTENSION;
                var assetName = targetFile.ReplaceFirst(_rootPath + "/", "");

                var index = ResBuildUtility.TryGetBuildInfo(filteredBuildInfos, assetBundleName);
                if (index == -1)
                {
                    var buildInfo = new AssetBundleBuild();
                    buildInfo.assetNames = new string[] { assetName };
                    buildInfo.assetBundleName = assetBundleName;
                    filteredBuildInfos.Add(buildInfo);

                    LogManager.Log("lua assetbundle " + assetBundleName);
                }
                else
                {
                    var buildInfo = filteredBuildInfos[index];
                    var assetNames = buildInfo.assetNames.ToList();
                    if (assetNames.Contains(assetName))
                        continue;

                    assetNames.Add(assetName);
                    buildInfo.assetNames = assetNames.ToArray();
                    filteredBuildInfos[index] = buildInfo;

                    LogManager.Log("lua assetbundle " + assetBundleName);
                }
            }
            AssetDatabase.Refresh();
        }

        public override void AfterBuild()
        {
            for (int i = 0; i < _tmpFiles.Count; ++i)
            {
                var tmpFile = _tmpFiles[i];

                var sourceFile = tmpFile;
                var targetFile = sourceFile.ReplaceLast(ResConfig.MOBILE_LUA_EXTENSION, ResConfig.LUA_EXTENSION);

                FileUtility.MoveFile(sourceFile, targetFile);
                FileUtility.MoveFile(sourceFile+".meta",targetFile+".meta");
            }
            AssetDatabase.Refresh();
        }
    }
}
