#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using TMS.Common.Extensions;
using TMS.Common.Helpers;
using TMS.Common.IO.Compression;

#endregion

namespace TMS.Common.Logging
{
	/// <summary>
	///     Unity Console Logger
	/// </summary>
	//[IocTypeMap(typeof(IFileLogger), true)] // TODO remove Debug -> Write to cache and file
	public class FileLogger : BaseLogger, IFileLogger
	{
		//protected const string DefaultSettingsFilePath = @"Config/FileLoggerConfig";

		/// <summary>
		///     Initializes a new instance of the <see cref="FileLogger" /> class.
		/// </summary>
		public FileLogger()
		{
			//SettingsFilePath = DefaultSettingsFilePath;

			FileStreams = new Dictionary<string, FileStreamWrapperBase>();
		}

		protected internal virtual IDictionary<string, FileStreamWrapperBase> FileStreams { get; set; }

		protected virtual string FilePath { get; set; }

		public virtual IFileLogger Init(string sid, FileLoggerSettings settings = null)
		{
			ArgumentValidator.AssertNotNullOrEmpty(sid, "sid");

			if (settings == null)
			{
				var dataPath = LocalStorage.Default.LocalDataFolderPath;
				var dir = Path.Combine(dataPath, "_LOG");

				settings = new FileLoggerSettings
				{
					IsEnabled = true,
					IsAutoTimeStampEnabled = true,
					LogTarget = LogTargetType.File,
					Severity = LogSeverityType.Normal,
					TimeStampFormat = "T",
					DefaultMessageFormat = "{0}",
					BackupTrigger = FileBackupTriggerType.MaxFileSize,
					DirectoryPath = dir,
					FileName = string.Format("{0}_", sid),
					FileExtension = "log",
					IsRecordsCacheEnabled = false,
					MaxFileSize = 2000000, // 2mb
					MaxRecordsCount = 10000,
					RecordsCacheSize = 100,
					RecordWriteTrigger = FileRecordWriteTriggerType.Critical,
					LineDelimiter = ",",
					OpeningLine = "[",
					ClosingLine = "]",
					UseGzipCompression = false,
					GzipFileExtension = "bak"
				};
			}
			Settings = settings;
			return this;
		}

		/// <summary>
		///     Gets or sets the settings.
		/// </summary>
		/// <value>
		///     The settings.
		/// </value>
		public new virtual FileLoggerSettings Settings { get; set; }

		/// <summary>
		///     Releases unmanaged and - optionally - managed resources.
		/// </summary>
		public override void Dispose()
		{
			base.Dispose();

			Flush();
			FileStreams = null;
		}

		public void LoadFromFile(string filePath, EventHandler<FileLoggerCallbackArgs> callback)
		{
			try
			{
				if (!Settings.IsEnabled)
				{
					const string errMsg =
						"reading from log file is aborted. Logger is disabled.\r\nTo enable logger set 'Settings.IsEnabled = true'.";
#if UNITY_3D || UNITY3D
					UnityEngine.Debug.LogWarning(errMsg);
#else
					Debug.WriteLine(errMsg);
#endif
					return;
				}

				ArgumentValidator.AssertNotNullOrEmpty(filePath, "filePath");

				if (!File.Exists(filePath))
					throw new FileNotFoundException(filePath);
				if (FileStreams.ContainsKey(filePath))
					throw new OperationCanceledException("File with path '" + filePath + "' is in process.");

				var wrapper = new FileStreamReader(filePath, callback)
				{
					IsFileCompressionEnabled = Settings.UseGzipCompression
				};
				wrapper.Callback += OnFileStreamAction;
				FileStreams.Add(filePath, wrapper);
				wrapper.LoadFromFile();
			}
			catch (Exception e)
			{
				FileStreamWrapperBase.InvokeFileActionEvent(callback, null, filePath, null, null, FileLoggerAction.Read,
					FileLoggerResult.Error, e);

				if (callback == null)
					throw;
			}
		}

		public virtual void SaveToFile(string text, EventHandler<FileLoggerCallbackArgs> callback)
		{
			try
			{
				if (!Settings.IsEnabled)
				{
					const string errMsg =
						"Writing to log file is aborted. Logger is disabled.\r\nTo enable logger set 'Settings.IsEnabled = true'.";
#if UNITY_3D || UNITY3D
					UnityEngine.Debug.LogWarning(errMsg);
#else
					Debug.WriteLine(errMsg);
#endif
					return;
				}

				ArgumentValidator.AssertNotNullOrEmpty(text, "text");

				if (!Directory.Exists(Settings.DirectoryPath))
					Directory.CreateDirectory(Settings.DirectoryPath);

				var wrapper = GetFileStream(callback);
				wrapper.SaveToFile(text);
			}
			catch (Exception e)
			{
				FileStreamWrapperBase.InvokeFileActionEvent(callback, null, FilePath, null, null, FileLoggerAction.Write,
					FileLoggerResult.Error, e);

				if (callback == null)
					throw;
			}
		}

		public void Flush()
		{
			if (FileStreams.IsNullOrEmpty()) return;
			try
			{
				FileStreams.Values.ForEach(file => file.Dispose());
			}
			catch (Exception e)
			{
				// TODO
			}
			finally
			{
				FileStreams.Clear();
				FilePath = null;
			}
		}

		public virtual string[] GetSavedLogFilePaths()
		{
			if (!Directory.Exists(Settings.DirectoryPath))
				return null;

			string[] files;
			var logFilePaths = LocalStorage.Default.GetFiles(Settings.DirectoryPath, "*." + Settings.FileExtension, false);
			var gzipFilePaths = LocalStorage.Default.GetFiles(Settings.DirectoryPath, "*." + Settings.GzipFileExtension, false);
			if (!logFilePaths.IsNullOrEmpty())
				files = !gzipFilePaths.IsNullOrEmpty() ? logFilePaths.Concat(gzipFilePaths).ToArray() : logFilePaths;
			else
				files = gzipFilePaths;
			return files;
		}

		public virtual void SaveToFile(EventHandler<FileLoggerCallbackArgs> callback)
		{
			throw new NotImplementedException();
		}

		public virtual void LoadFromFile(EventHandler<FileLoggerCallbackArgs> callback)
		{
			throw new NotImplementedException();
		}

		protected virtual string GetNewFilePath()
		{
			var filePath = string.Format("{0}{1}.{2}",
				Path.Combine(Settings.DirectoryPath, Settings.FileName),
				DateTime.UtcNow.ToFileTime(), Settings.FileExtension);
			return filePath;
		}

		public virtual void SaveToFile()
		{
			throw new NotImplementedException();
		}

		private void OnFileStreamAction(object sender, FileLoggerCallbackArgs e)
		{
			var wrapper = (FileStreamWrapperBase) sender;
			switch (e.Status)
			{
				case FileLoggerResult.Error:
				case FileLoggerResult.Aborted:
					Dispose(wrapper);
					break;

				case FileLoggerResult.Success:
					switch (e.Action)
					{
						case FileLoggerAction.Closing:
							var writer = wrapper as FileStreamWriter;
							if (writer != null && !Settings.ClosingLine.IsNullOrEmpty())
								writer.SaveToFile(Settings.ClosingLine);
							break;

						case FileLoggerAction.Closed:
						case FileLoggerAction.Compressed:
							Dispose(wrapper);
							break;
					}
					break;
			}
		}

		protected virtual void Dispose(FileStreamWrapperBase wrapper)
		{
			wrapper.Callback -= OnFileStreamAction;
			if (FileStreams.ContainsKey(wrapper.FilePath))
				FileStreams.Remove(wrapper.FilePath);
			wrapper.Dispose();
		}

		protected virtual FileStreamWriter GetFileStream(EventHandler<FileLoggerCallbackArgs> callback)
		{
			var wrapper = !FilePath.IsNullOrEmpty() && FileStreams.ContainsKey(FilePath)
				? FileStreams[FilePath] as FileStreamWriter
				: null;

			if (wrapper != null)
			{
				if (wrapper.FileStream.Length < Settings.MaxFileSize)
				{
					wrapper.SaveToFile(Settings.LineDelimiter);
					return wrapper;
				}
				FileStreams.Remove(wrapper.FilePath);
				wrapper.Close();
			}

			FilePath = GetNewFilePath();
			wrapper = new FileStreamWriter(FilePath, callback)
			{
				IsFileCompressionEnabled = Settings.UseGzipCompression,
				GzipFileExt = Settings.GzipFileExtension
			};
			wrapper.Callback += OnFileStreamAction;
			FileStreams.Add(FilePath, wrapper);

			if (!Settings.OpeningLine.IsNullOrEmpty())
				wrapper.SaveToFile(Settings.OpeningLine);
			return wrapper;
		}
	}

	#region File Stream Wrappers

	public abstract class FileStreamWrapperBase : IDisposable
	{
		internal const int MaxBufferSize = 24000;

		internal FileStream FileStream { get; set; }

		internal byte[] Buffer { get; set; }

		internal int BufferReadSize { get; set; }

		internal string FilePath { get; set; }

		internal bool IsFileCompressionEnabled { get; set; }

		internal abstract FileLoggerAction ActionType { get; }

		public abstract void Dispose();

		internal event EventHandler<FileLoggerCallbackArgs> Callback;

		internal abstract void Close();

		internal static void InvokeFileActionEvent(EventHandler<FileLoggerCallbackArgs> callback,
			object sender, string filePath, string text, byte[] data,
			FileLoggerAction action, FileLoggerResult status, Exception error = null)
		{
#if !UNITY_3D && !UNITY3D
			if (status != FileLoggerResult.Success)
				Debug.WriteLine("action: {0}, status: {1}\r\nerror:{2}", action, status, error);
#endif
			if (callback == null) return;
			callback(sender, new FileLoggerCallbackArgs
			{
				FilePath = filePath,
				Text = text,
				Data = data,
				Action = action,
				Status = status,
				Error = error
			});
		}

		internal void InvokeFileActionEvent(string filePath, string text, FileLoggerAction action, FileLoggerResult status,
			Exception error = null)
		{
			InvokeFileActionEvent(Callback, this, filePath, text, null, action, status, error);
		}

		internal void InvokeFileActionEvent(string filePath, byte[] data, FileLoggerAction action, FileLoggerResult status,
			Exception error = null)
		{
			InvokeFileActionEvent(Callback, this, filePath, null, data, action, status, error);
		}

		internal void InvokeFileActionEvent(string filePath, FileLoggerAction action, FileLoggerResult status,
			Exception error = null)
		{
			InvokeFileActionEvent(Callback, this, filePath, null, null, action, status, error);
		}
	}

	public class FileStreamReader : FileStreamWrapperBase
	{
		public FileStreamReader(string filePath, EventHandler<FileLoggerCallbackArgs> callback)
		{
			try
			{
				Callback += callback;
				ArgumentValidator.AssertNotNullOrEmpty(filePath, "filePath");

				if (!File.Exists(filePath))
					throw new FileNotFoundException(filePath);

				FilePath = filePath;
				FileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
				if (!FileStream.CanRead)
					throw new OperationCanceledException("Cannot read from " + filePath);
			}
			catch (Exception e)
			{
				InvokeFileActionEvent(filePath, FileLoggerAction.Init, FileLoggerResult.Error, e);
			}
		}

		internal override FileLoggerAction ActionType
		{
			get { return FileLoggerAction.Read; }
		}

		internal override void Close()
		{
			if (FileStream == null) return;

			FileStream.Dispose();
			FileStream = null;

			InvokeFileActionEvent(FilePath, Buffer, FileLoggerAction.Closed, FileLoggerResult.Success);
		}

		internal void LoadFromFile()
		{
			try
			{
				Buffer = new byte[FileStream.Length];

				var byteCount = FileStream.Length > MaxBufferSize ? MaxBufferSize : FileStream.Length;
				FileStream.BeginRead(Buffer, 0, (int) byteCount, OnFileRead, this);
			}
			catch (Exception e)
			{
				InvokeFileActionEvent(FilePath, FileLoggerAction.Read, FileLoggerResult.Error, e);
			}
		}

		private void OnFileRead(IAsyncResult ar)
		{
			try
			{
				var readCount = FileStream.EndRead(ar);
				BufferReadSize += readCount;

				if (BufferReadSize >= FileStream.Length)
				{
					Close();
					return;
				}

				if (BufferReadSize + readCount > Buffer.Length)
					readCount = Buffer.Length - BufferReadSize;

				FileStream.BeginRead(Buffer, BufferReadSize, readCount, OnFileRead, null);

				InvokeFileActionEvent(FileStream.Name, FileLoggerAction.Read, FileLoggerResult.Success);
			}
			catch (Exception e)
			{
				InvokeFileActionEvent(FileStream != null ? FileStream.Name : string.Empty, FileLoggerAction.Read,
					FileLoggerResult.Error, e);
			}
		}

		public override void Dispose()
		{
			Close();
		}
	}

	public class FileStreamWriter : FileStreamWrapperBase
	{
		private FileLoggerAction _currentAction;

		private bool _isWriting;

		public FileStreamWriter(string filePath, EventHandler<FileLoggerCallbackArgs> callback)
		{
			ArgumentValidator.AssertNotNullOrEmpty(filePath, "filePath");

			FilePath = filePath;
			FileStream = new FileStream(FilePath, FileMode.Append);
			Callback += callback;
		}

		internal override FileLoggerAction ActionType
		{
			get { return FileLoggerAction.Write; }
		}

		public string GzipFileExt { get; set; }

		internal override void Close()
		{
			if (FileStream == null || _currentAction == FileLoggerAction.Closed) return;

			if (_currentAction != FileLoggerAction.Closing)
			{
				_currentAction = FileLoggerAction.Closing;
				InvokeFileActionEvent(FilePath, FileLoggerAction.Closing, FileLoggerResult.Success);
			}
			if (_isWriting) return;

			try
			{
				//FileStream.Flush();
				FileStream.Dispose();
				FileStream = null;

				if (!IsFileCompressionEnabled)
				{
					InvokeFileActionEvent(FilePath, FileLoggerAction.Closed, FileLoggerResult.Success);
					return;
				}
				CompressFile();
			}
			catch (Exception e)
			{
				InvokeFileActionEvent(FilePath, FileLoggerAction.Closed, FileLoggerResult.Error, e);
			}
			finally
			{
				_currentAction = FileLoggerAction.Closed;
			}
		}

		private void CompressFile()
		{
			if (FilePath.IsNullOrEmpty() || !File.Exists(FilePath)) return;
			var thZip = new Thread(WriteZipFile);
			thZip.Start(FilePath);
		}

		private void WriteZipFile(object obj)
		{
			var filePath = obj as string;
			try
			{
				ArgumentValidator.AssertNotNullOrEmpty(filePath, "filePath");
				if (!File.Exists(filePath))
					throw new FileNotFoundException("Cannot compress file. Cannot locate file.", filePath);

				Thread.Sleep(40); // wait 1 frame

				var dirPath = Path.GetDirectoryName(filePath);
				var fileName = Path.GetFileName(filePath);
				var zipFileName = Path.ChangeExtension(fileName, GzipFileExt);
				GZipHelper.Default.CompressFile(dirPath, fileName, false, zipFileName, null);

				var zipFilePath = Path.Combine(dirPath, zipFileName);
				if (File.Exists(zipFilePath))
				{
					//System.Diagnostics.Debug.WriteLine("deleting " + filePath);
					File.Delete(filePath);
					//System.Diagnostics.Debug.WriteLine("deleted " + filePath);
					InvokeFileActionEvent(zipFilePath, FileLoggerAction.Compressed, FileLoggerResult.Success);
				}
			}
			catch (Exception e)
			{
				InvokeFileActionEvent(filePath, FileLoggerAction.Compressed, FileLoggerResult.Error, e);
			}
		}

		internal void SaveToFile(string text)
		{
			// TODO if _isWriting == true, should cache and to write after current write action is completed?
			_isWriting = true;
			try
			{
				ArgumentValidator.AssertNotNull(text, "text");

				// TODO define max buffer size and write large texts in chunks
				var buffer = Encoding.UTF8.GetBytes(text);
				FileStream.BeginWrite(buffer, 0, buffer.Length, OnFileWrite, buffer);
			}
			catch (Exception e)
			{
				_isWriting = false;
				InvokeFileActionEvent(FilePath, FileLoggerAction.Write, FileLoggerResult.Error, e);
			}
		}

		private void OnFileWrite(IAsyncResult ar)
		{
			try
			{
				if (FileStream != null && FileStream.CanWrite)
					FileStream.EndWrite(ar);

				InvokeFileActionEvent(FilePath, ar.AsyncState as byte[], FileLoggerAction.Write, FileLoggerResult.Success);
			}
			catch (Exception e)
			{
				InvokeFileActionEvent(FilePath, FileLoggerAction.Write, FileLoggerResult.Error, e);
			}
			finally
			{
				_isWriting = false;
				if (_currentAction == FileLoggerAction.Closing)
					Close();
			}
		}

		public override void Dispose()
		{
			_isWriting = false;
			Close();
		}
	}

	#endregion
}