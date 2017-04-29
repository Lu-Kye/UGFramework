using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UGFramework.Extension.String;

namespace UGFramework.Res
{
    public class ResLuaBuildFilter : ResAbstractBuildFilter
    {
        public override string FileExtension { get { return ".lua"; } }
        public ResLuaBuildFilter() : base() {}

        public override AssetBundleBuild PrepareBuild(string filepath)
        {
            var buildInfo = base.PrepareBuild(filepath);
            
            // Change lua file to text asset
            var buildingFile = filepath.ReplaceLast(this.FileExtension, ResConfig.MOBILE_LUA_EXTENSION);
            File.Move(filepath, buildingFile);

            buildInfo.assetBundleName = buildingFile.ReplaceFirst(_rootPath + "/Assets/" + ResConfig.RES_ROOT + "/", "") + ResConfig.BUNDLE_EXTENSION;
            buildInfo.assetNames = new string[] { buildingFile.ReplaceFirst(_rootPath + "/", "") };
            return buildInfo;
        }

        protected override bool IsOverrideBuild(string trackingFile, AssetBundleBuild buildInfo) { return true; }

        public override void DoOverrideBuild(string outpathRoot, List<AssetBundleBuild> filteredBuildInfos)
        {
            foreach (var buildInfo in _trackingBuildInfos.Values)
            {
                var filename = Path.GetFileName(buildInfo.assetBundleName);
                var directory = buildInfo.assetBundleName.ReplaceLast("/" + filename, "").ToLower(); 

                var targetPath = outpathRoot + "/" + directory;
                if (Directory.Exists(targetPath) == false)
                    Directory.CreateDirectory(targetPath);

                var targetFile = targetPath + "/" + filename.ReplaceLast(ResConfig.BUNDLE_EXTENSION, "").ToLower();
                if (File.Exists(targetFile))
                    File.Delete(targetFile);

                File.Copy(buildInfo.assetNames[0], targetFile);
            }
        }

        public override void AfterBuild()
        {
            base.AfterBuild();

            // Revert text asset to lua file
            foreach (var buildingFile in _trackingFiles)
            {
                var filepath = buildingFile.ReplaceLast(this.FileExtension, ResConfig.MOBILE_LUA_EXTENSION);
                File.Move(filepath, buildingFile);
            }
            AssetDatabase.Refresh();
        }
    }
}
