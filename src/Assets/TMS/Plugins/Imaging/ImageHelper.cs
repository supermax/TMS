#region

using System;
using System.Collections.Generic;
using System.IO;
using TMS.Common.Extensions;

#endregion

namespace TMS.Common.Imaging
{
	public class ImageHelper
	{
		private const string DELETE_FLAG = "DELETE_ME";
		private readonly ImageHelperEventArgs _args = new ImageHelperEventArgs();
		public event EventHandler<ImageHelperEventArgs> Progress = delegate { };

		public float ResizeImageFile(string srcFilePath, float scale, string destFilePath, int minImgHeight, int minImgWidth)
		{
			//using (var src = Image.FromFile(srcFilePath))
			//{
			//	var w = (int) Math.Round(src.Width*scale);
			//	var h = (int) Math.Round(src.Height*scale);

			//	if (w < minImgWidth || w < minImgHeight)
			//	{
			//		scale = (1f - scale/2.0f);
			//		w = (int) Math.Round(src.Width*scale);
			//		h = (int) Math.Round(src.Height*scale);
			//	}
			//	if (w <= 0) w = 1;
			//	if (h <= 0) h = 1;

			//	using (var dst = new Bitmap(src, w, h))
			//	{
			//		dst.Save(destFilePath, src.RawFormat);
			//	}
			//}
			//return scale;

			throw new NotImplementedException();
		}

		public IDictionary<string, float> ResizeImageFiles(string srcFolder, string filter, float scale,
			int minImgHeight, int minImgWidth)
		{
			var dic = new Dictionary<string, float>();
			if (srcFolder.Contains("Plugins")) return dic;
			_args.ProgressPercent = 0f;
			_args.ProgressMessage = string.Format("Resizing files in \"{0}\"...", srcFolder);
			Progress(this, _args);

			var progressCounter = 0;
			var files = Directory.GetFiles(srcFolder, filter, SearchOption.AllDirectories);
			if (files.IsNullOrEmpty())
			{
				_args.ProgressPercent = 0f;
				_args.ProgressMessage = string.Format("Error. Cannot retrieve images files from \"{0}\".", srcFolder);
				return dic;
			}

			foreach (var file in files)
			{
				_args.ProgressPercent = (float) Math.Round((++progressCounter)/(double) files.Length, 2);
				_args.ProgressMessage = string.Format("Resizing \"{0}\"...", file);
				Progress(this, _args);

				var newPath = string.Format("{0}.tmp", file);
				var finalScale = ResizeImageFile(file, scale, newPath, minImgHeight, minImgWidth);

				File.Delete(file);
				File.Move(newPath, file);

				_args.ProgressMessage = string.Format("Done resizing \"{0}\".", file);
				Progress(this, _args);

				if (file.Contains("Assets"))
				{
					var relativeFilePath = file.Substring(file.IndexOf("Assets", StringComparison.InvariantCulture));
					dic.Add(relativeFilePath, finalScale);
				}
			}

			_args.ProgressMessage = string.Format("Resized files in \"{0}\".", srcFolder);
			Progress(this, _args);
			return dic;
		}

		public static void CopyFolder(string sourceFolder, string destFolder, bool excludeFiles)
		{
			if (!IsCopyAllowed(sourceFolder) && excludeFiles) return;

			if (!Directory.Exists(destFolder))
			{
				Directory.CreateDirectory(destFolder);
			}

			var files = Directory.GetFiles(sourceFolder);
			foreach (var file in files)
			{
				var name = Path.GetFileName(file);
				var dest = Path.Combine(destFolder, name);
				File.Copy(file, dest, true);
			}

			var folders = Directory.GetDirectories(sourceFolder);
			foreach (var folder in folders)
			{
				if (!folder.ToLowerInvariant().Contains("facebook"))
				{
					var ignoreFolder = Path.GetFileNameWithoutExtension(folder.ToLowerInvariant());
					switch (ignoreFolder)
					{
						case "bin":
						case "builds":
						case "obj":
						case "temp":
							continue;
					}
				}
			var name = Path.GetFileName(folder);
				var dest = Path.Combine(destFolder, name);
				CopyFolder(folder, dest, excludeFiles);
			}
		}

		public static bool IsCopyAllowed(string sourceFolder)
		{
			var deleteFileFlag = Path.Combine(sourceFolder, DELETE_FLAG);
			var exists = File.Exists(deleteFileFlag);
			return !exists;
		}

		public static void DeleteFolder(string sourceFolder)
		{
			if (!Directory.Exists(sourceFolder))
			{
				return;
			}

			var files = Directory.GetFiles(sourceFolder);
			foreach (var file in files)
			{
				File.Delete(file);
			}

			var folders = Directory.GetDirectories(sourceFolder);
			foreach (var folder in folders)
			{
				DeleteFolder(folder);
			}

			Directory.Delete(sourceFolder);
		}
	}

	public class ImageHelperEventArgs : EventArgs
	{
		public ImageHelperEventArgs()
		{
		}

		public ImageHelperEventArgs(float percent, string msg)
		{
			ProgressPercent = percent;
			ProgressMessage = msg;
		}

		public float ProgressPercent { get; protected internal set; }

		public string ProgressMessage { get; protected internal set; }
	}
}