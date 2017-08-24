using UnityEngine;
using UnityEditor;
using System.IO;

namespace UGFramework.Res
{
	public class ResTextureAutoConfigurer : AssetPostprocessor 
	{
	    void OnPostprocessTexture(Texture2D texture) 
		{
			if (this.assetPath.StartsWith("Assets/" + ResConfig.UI_TEXTURE_RUNTIME) == false)
			{
				return;
			}
			this.ConfigureUITexture(texture);
		}
	
		void ConfigureUITexture(Texture2D texture)
		{
			var textureImporter = this.assetImporter as TextureImporter;
			ConfigureUITexture(textureImporter);
		}
	
		public static void ConfigureUITexture(TextureImporter importer, bool clear = false)
		{
			string atlasName = new DirectoryInfo(Path.GetDirectoryName(importer.assetPath)).Name;
			importer.npotScale = TextureImporterNPOTScale.None;
			importer.textureType = TextureImporterType.Sprite;
			importer.spritePackingTag = clear ? "" : atlasName;
			importer.mipmapEnabled = false;
		}
	}
}