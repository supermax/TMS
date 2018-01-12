#define UNITY3D

using System;
using TMS.Common.Serialization.Json;
using UnityEngine;

namespace TMS.Common.Logging.Api
{
	/// <summary>
	///     File Logger Settings
	/// </summary>
	[JsonDataContract]
	[Serializable]
	public class FileLoggerSettings : LoggerSettings
	{
		/// <summary>
		///     Initializes the <see cref="FileLoggerSettings" /> class.
		/// </summary>
		static FileLoggerSettings()
		{
			JsonMapper.Default.RegisterImporter<string, FileBackupTriggerType>(
				input => (FileBackupTriggerType) Enum.Parse(typeof (FileBackupTriggerType), input));

			JsonMapper.Default.RegisterImporter<string, FileRecordWriteTriggerType>(
				input => (FileRecordWriteTriggerType) Enum.Parse(typeof (FileRecordWriteTriggerType), input));
		}

		/// <summary>
		///     Gets or sets the name of the file.
		/// </summary>
		/// <value>
		///     The name of the file.
		/// </value>
		[JsonDataMember("fileName")]
		public string FileName { get { return _fileName; } set { _fileName = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private string _fileName;

		/// <summary>
		///     Gets or sets the file extension.
		/// </summary>
		/// <value>
		///     The file extension.
		/// </value>
		[JsonDataMember("fileExt")]
		public string FileExtension { get { return _fileExt; } set { _fileExt = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private string _fileExt;

		[JsonDataMember("gZipFileExt")]
		public string GzipFileExtension { get { return _gZipFileExt; } set { _gZipFileExt = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private string _gZipFileExt;

		/// <summary>
		///     Gets or sets the directory path.
		/// </summary>
		/// <value>
		///     The directory path.
		/// </value>
		[JsonDataMember("dirPath")]
		public string DirectoryPath { get { return _dirPath; } set { _dirPath = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private string _dirPath;

		/// <summary>
		///     Gets or sets the maximum size of the file.
		/// </summary>
		/// <value>
		///     The maximum size of the file.
		/// </value>
		[JsonDataMember("maxFileSize")]
		public long MaxFileSize { get { return _maxFileSize; } set { _maxFileSize = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private long _maxFileSize;

		/// <summary>
		///     Gets or sets the maximum records count.
		/// </summary>
		/// <value>
		///     The maximum records count.
		/// </value>
		[JsonDataMember("maxRecCount")]
		public int MaxRecordsCount { get { return _maxRecCount; } set { _maxRecCount = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private int _maxRecCount;

		/// <summary>
		///     Gets or sets the backup trigger.
		/// </summary>
		/// <value>
		///     The backup trigger.
		/// </value>
		[JsonDataMember("bkpTrigger")]
		public FileBackupTriggerType BackupTrigger { get { return _bkpTrigger; } set { _bkpTrigger = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private FileBackupTriggerType _bkpTrigger;

		/// <summary>
		///     Gets or sets the record write trigger.
		/// </summary>
		/// <value>
		///     The record write trigger.
		/// </value>
		[JsonDataMember("recTrigger")]
		public FileRecordWriteTriggerType RecordWriteTrigger { get { return _recTrigger; } set { _recTrigger = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private FileRecordWriteTriggerType _recTrigger;

		/// <summary>
		/// Gets or sets the opening line.
		/// </summary>
		/// <value>
		/// The opening line.
		/// </value>
		[JsonDataMember("openingLine", "[")]
		public string OpeningLine
		{
			get { return _openingLine; }
			set { _openingLine = value; }
		}

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private string _openingLine;

		/// <summary>
		/// Gets or sets the closing line.
		/// </summary>
		/// <value>
		/// The closing line.
		/// </value>
		[JsonDataMember("closingLine", "]")]
		public string ClosingLine
		{
			get { return _closingLine; }
			set { _closingLine = value; }
		}

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private string _closingLine;

		/// <summary>
		/// Gets or sets the line delimiter.
		/// </summary>
		/// <value>
		/// The line delimiter.
		/// </value>
		[JsonDataMember("lineDelimiter", ",")]
		public string LineDelimiter
		{
			get { return _lineDelimiter; }
			set { _lineDelimiter = value; }
		}

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private string _lineDelimiter;

		/// <summary>
		/// Gets or sets a value indicating whether [use GZIP compression].
		/// </summary>
		/// <value>
		///   <c>true</c> if [use GZIP compression]; otherwise, <c>false</c>.
		/// </value>
		[JsonDataMember("useGzip", DefaultValue = true, FallbackValue = true)]
		public bool UseGzipCompression
		{
			get { return _useGzip; }
			set { _useGzip = value; }
		}

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private bool _useGzip;
	}
}