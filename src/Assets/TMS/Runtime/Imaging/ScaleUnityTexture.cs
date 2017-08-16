using System;
using System.IO;
using UnityEngine;

namespace TMS.Common.Imaging
{
	public class ScaleUnityTexture
	{
		public enum ImageFormat
		{
			Unknown,
			Png,
			Jpg
		}

		public enum ScaleType
		{
			Point,
			Linear,
			Lanczos
		}

		public static Color32[] ScaleLinear(Color32[] bytes, int width, int targetWidth, int targetHeight)
		{
			var colors = GetColors(bytes);
			var si = new ScaleImage(colors, width);
			colors = si.ScaleLinear(targetWidth, targetHeight);
			
			return GetColors(colors);
		}

		public static Color32[] ScaleLanczos(Color32[] bytes, int width, int targetWidth, int targetHeight)
		{
			var colors = GetColors(bytes);
			var l = new ScaleImage(colors, width);
			colors = l.ScaleLanczos(targetWidth, targetHeight);
			return GetColors(colors);
		}

		public static Color32[] ScalePoint(Color32[] bytes, int width, int targetWidth, int targetHeight)
		{
			var colors = GetColors(bytes);

			var si = new ScaleImage(colors, width);
			colors = si.ScalePoint(targetWidth, targetHeight);
			
			return GetColors(colors);
		}

		public static Color32[] GetColors(SColor[] colors)
		{
			var outArray = new Color32[colors.Length];
			for (var i = 0; i < colors.Length; i++)
			{
				outArray[i] = new Color32(
					(byte) (colors[i].R*255),
					(byte) (colors[i].G*255),
					(byte) (colors[i].B*255),
					(byte) (colors[i].A*255)
					);
			}
			return outArray;
		}

		public static SColor[] GetColors(Color32[] bytes)
		{
			var outArray = new SColor[bytes.Length];
			for (var i = 0; i < bytes.Length; i++)
			{
				outArray[i] = new SColor(
					bytes[i].r/255.0f,
					bytes[i].g/255.0f,
					bytes[i].b/255.0f,
					bytes[i].a/255.0f
					);
			}
			return outArray;
		}

		public static float Scale(ScaleType type, string path, string target, float scale, ImageFormat imageFormat,
			int minImgHeight, int minImgWidth)
		{
			//
			// load the texture from the disc by 
			// setting the importer to use RGBA32 and ensure the texture is readable
			//

			//var ti = AssetImporter.GetAtPath(path) as TextureImporter;
			//var orgSettings = new TextureImporterSettings();
			//
			//ti.ReadTextureSettings(orgSettings);
			//
			//ti.textureFormat = TextureImporterFormat.RGBA32;
			//ti.isReadable = true;
			//ti.maxTextureSize = 4096;
			//ti.npotScale = TextureImporterNPOTScale.None;
			//AssetDatabase.ImportAsset(path);

			// get the pixels
			//var originalTexture = AssetDatabase.LoadAssetAtPath(path, typeof (Texture2D)) as Texture2D;
			var orgBytes = File.ReadAllBytes(path);
			var orgTexture = new Texture2D(0,0);
			orgTexture.LoadImage(orgBytes);
			
			var orgWidth = orgTexture.width;
			var orgHeight = orgTexture.height;
			//originalTexture = null;

			//
			// restore original import settings
			//

			//ti = AssetImporter.GetAtPath(path) as TextureImporter;
			//ti.SetTextureSettings(orgSettings);
			//AssetDatabase.ImportAsset(path);


			// arbitrary target size
			//int width = size;
			//preserve aspect ratio
			//int height = Mathf.RoundToInt((width / (float)orgWidth) * orgHeight);

			var w = (int)Math.Round(orgWidth * scale);
			var h = (int)Math.Round(orgHeight * scale);

			if (w < minImgWidth || w < minImgHeight)
			{
				scale = (1f - scale / 2.0f);
				w = (int)Math.Round(orgWidth * scale);
				h = (int)Math.Round(orgHeight * scale);
			}
			if (w <= 0) w = 1;
			if (h <= 0) h = 1;

			//actually scale the data
			var c1 = orgTexture.GetPixels32();
			switch (type)
			{
				case ScaleType.Lanczos:
					c1 = ScaleLanczos(c1, orgWidth, w, h);
					break;

				case ScaleType.Linear:
					c1 = ScaleLinear(c1, orgWidth, w, h);
					break;

				case ScaleType.Point:
					c1 = ScalePoint(c1, orgWidth, w, h);
					break;
			}

			Texture2D outT;
			byte[] outBytes = null;
			switch (imageFormat)
			{
				case ImageFormat.Png:
					// create target texture
					outT = new Texture2D(w, h, TextureFormat.ARGB32, false);
					// set the pixels
					outT.SetPixels32(c1);
					// encode the texture
					outBytes = outT.EncodeToPNG();
					//Debug.LogWarning("saving " + target + " as png");
					break;

				case ImageFormat.Jpg:
					// create target texture
					outT = new Texture2D(w, h, TextureFormat.RGB24, false);
					// set the pixels
					outT.SetPixels32(c1);
					// encode the texture
					outBytes = outT.EncodeToJPG();
					//Debug.LogWarning("saving " + target + " as jpg");
					break;
			}

			// save texture
			File.WriteAllBytes(target, outBytes);
			//AssetDatabase.ImportAsset(newPath);
			//AssetDatabase.Refresh();
			// Apply the same import settings for this texture
			//ti = AssetImporter.GetAtPath(newPath) as TextureImporter;
			//ti.SetTextureSettings(orgSettings);
			//AssetDatabase.ImportAsset(newPath);
			//AssetDatabase.Refresh();

			return scale;
		}
	}
}