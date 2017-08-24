using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UGFramework.Extension;

namespace UGFramework.Res
{
    public class ResSpriteBuildFilter : ResAbstractBuildFilter 
    {
    	public override string FileExtension 
        { 
            get 
            { 
                return ".png"; 
            } 
        }
        public ResSpriteBuildFilter() : base() { }
    
        public override void BeforeBuild()
        {
            base.BeforeBuild();
    
            // Clear altas settings
            var resPath = Application.dataPath + "/" + ResConfig.UI_TEXTURE;
            var files = ResBuildUtility.GetFiles(resPath);
            foreach (var file in files)
            {
                if (file.EndsWith(".png") == false && file
                .EndsWith(".jpg") == false)
                    continue;
    
                var assetPath = "Assets/" + file.ReplaceFirst(Application.dataPath + "/", "");
                if (assetPath.StartsWith("Assets/" + ResConfig.UI_TEXTURE_RUNTIME))
                    continue;
    
                var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                ResTextureAutoConfigurer.ConfigureUITexture(textureImporter, true);
            }
        }
    
        public override void OverrideBuild(string outpathRoot, List<AssetBundleBuild> filteredBuildInfos)
        {
            for (int i = filteredBuildInfos.Count - 1; i >= 0; --i)
            {
                var buildInfo = filteredBuildInfos[i];
                // Remove runtime textures from buildInfos
                if (buildInfo.assetBundleName.StartsWith("Assets/" + ResConfig.UI_TEXTURE_RUNTIME) ||
                    buildInfo.assetBundleName.StartsWith(ResConfig.DEPENDENCIES_ROOT + "/" + ResConfig.UI_TEXTURE_RUNTIME))
                {
                    filteredBuildInfos.RemoveAt(i);
                }
            }
        
            var resPath = Application.dataPath + "/" + ResConfig.UI_TEXTURE_RUNTIME;
            var files = ResBuildUtility.GetFiles(resPath);
    
            foreach (var file in files)
            {
                if (file.EndsWith(".png") == false && 
                    file.EndsWith(".jpg") == false)
                    continue;
                
                var fileName = Path.GetFileName(file);
                var assetBundleName = file.ReplaceFirst(_rootPath + "/", "");
                assetBundleName = assetBundleName.ReplaceLast("/" + fileName, "").ReplaceFirst("Assets/", "") + ResConfig.BUNDLE_EXTENSION;
                var assetName = file.ReplaceFirst(_rootPath + "/", "");
    
                var index = ResBuildUtility.TryGetBuildInfo(filteredBuildInfos, assetBundleName);
                if (index == -1)
                {
                    var buildInfo = new AssetBundleBuild();
                    buildInfo.assetNames = new string[] { assetName };
                    buildInfo.assetBundleName = assetBundleName;
                    filteredBuildInfos.Add(buildInfo);
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
                }
            }
        }
    }
}