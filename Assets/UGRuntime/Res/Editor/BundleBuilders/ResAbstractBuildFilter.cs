using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UGFramework.Res
{
    public abstract class ResAbstractBuildFilter
    {
        public virtual string FileExtension { get { return "unknown"; } }
        public virtual bool BlockOthers { get { return true; } }

        protected static string _rootPath = Application.dataPath.ReplaceLast("/Assets", "");
        protected List<string> _trackingFiles = new List<string>();
        protected Dictionary<string, AssetBundleBuild> _trackingBuildInfos = new Dictionary<string, AssetBundleBuild>();

        public virtual void BeforeBuild()
        {
            _trackingFiles.Clear();
            _trackingBuildInfos.Clear();
        }

        public virtual bool Filtered(string filepath)
        {
            return filepath.EndsWith(this.FileExtension);
        }

        public virtual AssetBundleBuild PrepareBuild(string filepath)
        {
            _trackingFiles.Add(filepath); 

            var buildInfo = new AssetBundleBuild();
            buildInfo.assetBundleName = filepath.ReplaceFirst(_rootPath + "/Assets/" + ResConfig.RES_ROOT + "/", "") + ResConfig.BUNDLE_EXTENSION;
            buildInfo.assetNames = new string[] { filepath.ReplaceFirst(_rootPath + "/", "") };
            return buildInfo;
        }

        public bool OverrideBuild(string filepath, AssetBundleBuild buildInfo)
        {
            if (_trackingFiles.Contains(filepath) == false)
                return false;
            var isOverrideBuild = this.IsOverrideBuild(filepath, buildInfo);
            if (isOverrideBuild)
                _trackingBuildInfos[filepath] = buildInfo;
            return isOverrideBuild;
        }

        protected virtual bool IsOverrideBuild(string trackingFile, AssetBundleBuild buildInfo)
        {
            return false;
        }

        public virtual void DoOverrideBuild(string outpathRoot, List<AssetBundleBuild> filteredBuildInfos)
        {
        }

        public virtual void AfterBuild()
        {
        }
    }
}
