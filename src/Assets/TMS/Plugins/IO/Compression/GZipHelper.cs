#region Usings

using System;
using System.IO;
using System.Text;
using TMS.Common.Core;
using TMS.Common.Extensions;
#if UNITY3D || UNITY_3D
//https://github.com/Hitcents/Unity.IO.Compression
using Unity.IO.Compression;
#else
using System.IO.Compression;
#endif

#endregion

namespace TMS.Common.IO.Compression
{
	/// <summary>
	///     GZip Helper
	/// </summary>
	public class GZipHelper : Singleton<GZipHelper>
	{
		/// <summary>
		/// </summary>
		/// <param name="percent">The percent.</param>
		/// <param name="progressText">The progress text.</param>
		/// <returns></returns>
		public delegate bool ReportProgressDelegate(int percent, string progressText);

		/// <summary>
		/// Compresses the file.
		/// </summary>
		/// <param name="dirPath">The dir path.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="embedFileName">if set to <c>true</c> [embed file name].</param>
		/// <param name="zipFileName">Name of the zip file.</param>
		/// <param name="progress">The progress.</param>
		public void CompressFile(string dirPath, string fileName, bool embedFileName, string zipFileName, ReportProgressDelegate progress)
		{
			var zipFilePath = Path.Combine(dirPath, zipFileName);
			using (var outFile = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (var str = new GZipStream(outFile, CompressionMode.Compress))
				{
					CompressFile(dirPath, fileName, embedFileName, str, progress);
				}
				//outFile.Flush();
			}
		}

		/// <summary>
		/// Compresses the file.
		/// </summary>
		/// <param name="dirPath">The dir path.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="embedFileName">if set to <c>true</c> [embed file name].</param>
		/// <param name="zipStream">The zip stream.</param>
		/// <param name="progress">The progress.</param>
		public void CompressFile(string dirPath, string fileName, bool embedFileName, GZipStream zipStream, ReportProgressDelegate progress)
		{
			//Compress file name
			if (embedFileName)
			{
				var chars = fileName.ToCharArray();
				zipStream.Write(BitConverter.GetBytes(chars.Length), 0, sizeof(int));
				foreach (var c in chars)
				{
					zipStream.Write(BitConverter.GetBytes(c), 0, sizeof(char));
				}
			}

			//Compress file content
			var srcFilePath = Path.Combine(dirPath, fileName);
			var bytes = File.ReadAllBytes(srcFilePath);

			zipStream.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
			zipStream.Write(bytes, 0, bytes.Length);
		}

		/// <summary>
		/// Decompresses the file.
		/// </summary>
		/// <param name="dirPath">The dir path.</param>
		/// <param name="zipFilePath">The zip file path.</param>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="progress">The progress.</param>
		/// <returns></returns>
		public bool DecompressFile(string dirPath, string zipFilePath, string fileName, ReportProgressDelegate progress)
		{
			using (var fileStr = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
			{
				using (var zipStream = new GZipStream(fileStr, CompressionMode.Decompress, false))
				{
					var res = DecompressFile(dirPath, zipStream, fileName, progress);
					return res;
				}
			}
		}

		/// <summary>
		/// Decompresses the file.
		/// </summary>
		/// <param name="dirPath">The dir path.</param>
		/// <param name="zipStream">The zip stream.</param>
		/// <param name="sFileName">Name of the s file.</param>
		/// <param name="progress">The progress.</param>
		/// <returns></returns>
		public bool DecompressFile(string dirPath, GZipStream zipStream, string sFileName, ReportProgressDelegate progress)
		{
			byte[] bytes = null;
			//Decompress file name
			if (sFileName.IsNullOrEmpty())
			{
				bytes = new byte[sizeof(int)];
				var done = zipStream.Read(bytes, 0, sizeof(int));
				if (done < sizeof(int)) return false;

				var iNameLen = BitConverter.ToInt32(bytes, 0);
				bytes = new byte[sizeof(char)];
				var sb = new StringBuilder();
				for (var i = 0; i < iNameLen; i++)
				{
					if (progress != null)
					{
						var cancel = progress(0, null);
						if (cancel) return false;
					}

					zipStream.Read(bytes, 0, sizeof(char));
					var c = BitConverter.ToChar(bytes, 0);
					sb.Append(c);
				}

				sFileName = sb.ToString();
				if (progress != null)
				{
					var cancel = progress(0, sFileName);
					if (cancel) return false;
				}
			}
			else
			{
				if (progress != null)
				{
					var cancel = progress(0, sFileName);
					if (cancel) return false;
				}
			}

			//Decompress file content
			bytes = new byte[sizeof(int)];
			zipStream.Read(bytes, 0, sizeof(int));
			var iFileLen = BitConverter.ToInt32(bytes, 0);

			bytes = new byte[iFileLen];
			zipStream.Read(bytes, 0, bytes.Length);

			var filePath = Path.Combine(dirPath, sFileName);
			var finalDir = Path.GetDirectoryName(filePath);

			var dirExists = Directory.Exists(finalDir);
			if (!dirExists)
			{
				if (progress != null)
				{
					var cancel = progress(0, string.Format("Creating directory ({0}).", finalDir));
					if (cancel) return false;
				}

				Directory.CreateDirectory(finalDir);
				if (progress != null)
				{
					var cancel = progress(0, string.Format("Directory created ({0}).", finalDir));
					if (cancel) return false;
				}
			}

			using (var outFile = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				outFile.Write(bytes, 0, iFileLen);
				outFile.Flush();
			}
			return true;
		}

		/// <summary>
		///     Compresses the directory.
		/// </summary>
		/// <param name="dirPath">The dir path.</param>
		/// <param name="zipFilePath">The zip file path.</param>
		/// <param name="progress">The progress.</param>
		public void CompressDirectory(string dirPath, string zipFilePath, ReportProgressDelegate progress)
		{
			var files = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
			var dirLen = dirPath[dirPath.Length - 1] == Path.DirectorySeparatorChar ? dirPath.Length : dirPath.Length + 1;

			using (var outFile = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				using (var str = new GZipStream(outFile, CompressionMode.Compress))
				{
					foreach (var file in files)
					{
						if (progress != null)
						{
							var cancel = progress(0, null);
							if (cancel) return;
						}

						var relativePath = file.Substring(dirLen);
						CompressFile(dirPath, relativePath, true, str, progress);
					}
				}
				outFile.Flush();
			}
		}

		/// <summary>
		///     Decompresses to directory.
		/// </summary>
		/// <param name="zipFilePath">The zip file path.</param>
		/// <param name="outputDirPath">The output dir path.</param>
		/// <param name="progress">The progress.</param>
		public void DecompressToDirectory(string zipFilePath, string outputDirPath, ReportProgressDelegate progress)
		{
			using (var fileStr = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.None))
			{
				using (var zipStream = new GZipStream(fileStr, CompressionMode.Decompress, true))
				{
					while (DecompressFile(outputDirPath, zipStream, null, progress))
					{
						if (progress == null) continue;
						var cancel = progress(0, null);
						if (cancel) return;
					}
				}
			}
		}

		/// <summary>
		///     Decompresses to directory.
		/// </summary>
		/// <param name="ary">The ary.</param>
		/// <param name="outputDirPath">The output dir path.</param>
		/// <param name="progress">The progress.</param>
		public void DecompressToDirectory(byte[] ary, string outputDirPath, ReportProgressDelegate progress)
		{
			using (var memStr = new MemoryStream(ary))
			{
				using (var zipStream = new GZipStream(memStr, CompressionMode.Decompress, true))
				{
					while (DecompressFile(outputDirPath, zipStream, null, progress))
					{
						if (progress == null) continue;
						var cancel = progress(0, null);
						if (cancel) return;
					}
				}
			}
		}
	}
}