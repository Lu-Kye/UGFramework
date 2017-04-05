using UnityEngine;
using UnityEditor;
using System.IO;

namespace UGFramework.Res
{
	public class ResTextureAutoConfigurer : AssetPostprocessor 
	{
		const string _ART_ROOT = "Assets/Art";

		void OnPostprocessTexture(Texture2D texture) 
		{
			// Only deal with our art project textures
			if (this.assetPath.StartsWith(_ART_ROOT) == false)
				return;

			if (this.assetPath.StartsWith(_ART_ROOT + "/UI/Texture/"))
			{
				this.ConfigureUITexture(texture);
			}
			else
			{
				this.ConfigureTexture(texture);
			}
		}

		void ConfigureUITexture(Texture2D texture)
		{
			string AtlasName =  new DirectoryInfo(Path.GetDirectoryName(assetPath)).Name;
			TextureImporter textureImporter  = this.assetImporter as TextureImporter;
			textureImporter.textureType = TextureImporterType.Sprite;
			textureImporter.spritePackingTag = AtlasName;
			textureImporter.mipmapEnabled = false;
		}

		void ConfigureTexture(Texture2D texture)
		{
		}
	}
}