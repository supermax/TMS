#region Usings

using System;
using System.Collections.Generic;
using TMS.Common.Core;
using TMS.Common.Extensions;
using TMS.Common.Serialization.Json;

#if UNITY || UNITY3D
using UnityEngine;
#endif

#endregion

namespace TMS.Common.Logging
{

	#region Interfaces

	/// <summary>
	///     Interface for Logger
	/// </summary>
	public interface ILogger : IDisposable
	{
		/// <summary>
		///     Gets or sets the settings.
		/// </summary>
		/// <value>
		///     The settings.
		/// </value>
		LoggerSettings Settings { get; set; }

		/// <summary>
		///     Gets or sets the settings file path.
		/// </summary>
		/// <value>
		///     The settings file path.
		/// </value>
		string SettingsFilePath { get; set; }

		/// <summary>
		///     Gets or sets the records.
		/// </summary>
		/// <value>
		///     The records.
		/// </value>
		List<LogRecord> Records { get; set; }

		ILogger Write(LogSourceType src, Delegate method);

		/// <summary>
		///     Writes the specified record.
		/// </summary>
		/// <param name="record">The record.</param>
		ILogger Write(LogRecord record);

		/// <summary>
		///     Writes the specified record.
		/// </summary>
		/// <param name="src">The source type.</param>
		/// <param name="msgFormat">The MSG format.</param>
		/// <param name="args">The arguments.</param>
		ILogger Write(LogSourceType src, string msgFormat, params object[] args);

		/// <summary>
		///     Writes the specified records.
		/// </summary>
		/// <param name="records">The records.</param>
		ILogger Write(IEnumerable<LogRecord> records);

		/// <summary>
		///     Writes the specified MSG.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="msg">The MSG.</param>
		ILogger Write(LogSourceType src, string msg);

		/// <summary>
		///     Writes the specified MSG.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <returns></returns>
		ILogger Write(string msg);

		/// <summary>
		///     Writes the specified MSG with given format and args.
		/// </summary>
		/// <param name="msgFormat">The MSG format.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		ILogger Write(string msgFormat, params object[] args);

		/// <summary>
		///     Writes the specified exception.
		/// </summary>
		/// <param name="exc">The exception.</param>
		ILogger Write(Exception exc);

		/// <summary>
		///     Initializes this instance.
		/// </summary>
		/// <returns>Instance of logger</returns>
		ILogger Init();

		/// <summary>
		///     Clears the log.
		/// </summary>
		ILogger Clear();
	}

	/// <summary>
	///     Interface for Console Logger
	/// </summary>
	public interface IConsoleLogger : ILogger
	{
	}

	/// <summary>
	///     Interface for File Logger
	/// </summary>
	public interface IFileLogger : ILogger
	{
		/// <summary>
		///     Gets the settings.
		/// </summary>
		/// <value>
		///     The settings.
		/// </value>
		new FileLoggerSettings Settings { get; set; }

		IFileLogger Init(string sid, FileLoggerSettings settings = null);

		/// <summary>
		///     Saves to file.
		/// </summary>
		void SaveToFile(EventHandler<FileLoggerCallbackArgs> callback);

		/// <summary>
		///     Loads from file.
		/// </summary>
		void LoadFromFile(EventHandler<FileLoggerCallbackArgs> callback);

		void LoadFromFile(string filePath, EventHandler<FileLoggerCallbackArgs> callback);

		/// <summary>
		///     Saves to file.
		/// </summary>
		void SaveToFile(string text, EventHandler<FileLoggerCallbackArgs> callback);

		void Flush();

		string[] GetSavedLogFilePaths();
	}

	public class FileLoggerCallbackArgs : EventArgs
	{
		public virtual string FilePath { get; protected internal set; }

		public virtual FileLoggerAction Action { get; protected internal set; }

		public virtual FileLoggerResult Status { get; protected internal set; }

		public virtual Exception Error { get; protected internal set; }

		public virtual string Text { get; protected internal set; }

		public virtual byte[] Data { get; protected internal set; }

		public override string ToString()
		{
			return string.Format("FilePath: '{0}', Action: '{1}', Status: '{2}', Error: '{3}', Text (Length): {4}, Data (Length): {5}", 
				FilePath, Action, Status, Error, Text != null ? Text.Length : 0, Data != null ? Data.Length : 0);
		}

		protected internal FileLoggerCallbackArgs()
		{
				
		}

		protected internal FileLoggerCallbackArgs(string filePath, FileLoggerAction action, FileLoggerResult status, Exception error = null)
		{
			FilePath = filePath;
			Action = action;
			Status = status;
			Error = error;
		}

		protected internal FileLoggerCallbackArgs(string text, string filePath, 
			FileLoggerAction action, FileLoggerResult status, Exception error = null) : 
			this(filePath, action, status, error)
		{
			Text = text;
		}

		protected internal FileLoggerCallbackArgs(byte[] data, string filePath, 
			FileLoggerAction action, FileLoggerResult status, Exception error = null) : this(filePath, action, status, error)
		{
			Data = data;
		}
	}

	public enum FileLoggerResult
	{
		Success,
		Error,
		Aborted
	}

	public enum FileLoggerAction
	{
		Init,
		Read,
		Write,
		Closed,
		Closing,
		Compressed,
		Compressing
	}

	/// <summary>
	///     Interface for Remote File Logger
	/// </summary>
	public interface IRemoteFileLogger : IFileLogger
	{
		/// <summary>
		///     Gets the settings.
		/// </summary>
		/// <value>
		///     The settings.
		/// </value>
		new RemoteFileLoggerSettings Settings { get; }

		/// <summary>
		///     Sends to server.
		/// </summary>
		void SendToServer();
	}

	#endregion

	#region Classes\Payloads

	/// <summary>
	///     Remote File Logger Settings
	/// </summary>
	[JsonDataContract]
	[Serializable]
	public class RemoteFileLoggerSettings : FileLoggerSettings
	{
		/// <summary>
		///     Gets or sets the send trigger.
		/// </summary>
		/// <value>
		///     The send trigger.
		/// </value>
		[JsonDataMember("sndTrigger")]
		public FileSendTriggerType SendTrigger { get; set; }
	}

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

	/// <summary>
	///     Logger Settings
	/// </summary>
	[JsonDataContract]
	[Serializable]
	public class LoggerSettings
	{
		/// <summary>
		///     Initializes the <see cref="LoggerSettings" /> class.
		/// </summary>
		static LoggerSettings()
		{
			JsonMapper.Default.RegisterImporter<string, LogTargetType>(
				input => (LogTargetType) Enum.Parse(typeof (LogTargetType), input));

			JsonMapper.Default.RegisterImporter<string, LogSeverityType>(
				input => (LogSeverityType) Enum.Parse(typeof (LogSeverityType), input));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LoggerSettings"/> class.
		/// </summary>
		/// <summary>
		/// Gets or sets a value indicating whether this instance is enabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonDataMember("enabled", DefaultValue = true, FallbackValue = true)]
		public bool IsEnabled { get { return _enabled; } set { _enabled = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private bool _enabled = true;

		/// <summary>
		///     Gets the type of the target.
		/// </summary>
		/// <value>
		///     The type of the target.
		/// </value>
		[JsonDataMember("logTarget")]
		public LogTargetType LogTarget { get { return _logTarget;  } set { _logTarget = value;  } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private LogTargetType _logTarget;

		/// <summary>
		///     Gets or sets the severity.
		/// </summary>
		/// <value>
		///     The severity.
		/// </value>
		[JsonDataMember("logSeverity")]
		public LogSeverityType Severity { get { return _logSeverity; } set { _logSeverity = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private LogSeverityType _logSeverity;

		/// <summary>
		///     Gets or sets the time stamp format.
		/// </summary>
		/// <value>
		///     The time stamp format.
		/// </value>
		[JsonDataMember("timeStampFormat")]
		public string TimeStampFormat { get { return _timeStampFormat; } set { _timeStampFormat = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private string _timeStampFormat;

		/// <summary>
		///     Gets or sets a value indicating whether this instance is automatic time stamp enabled.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is automatic time stamp enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonDataMember("autoTimeStamp")]
		public bool IsAutoTimeStampEnabled { get { return _autoTimeStamp; } set { _autoTimeStamp = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private bool _autoTimeStamp;

		/// <summary>
		///     Gets or sets the default message format.
		/// </summary>
		/// <value>
		///     The default message format.
		/// </value>
		[JsonDataMember("msgFormat")]
		public string DefaultMessageFormat { get { return _msgFormat; } set { _msgFormat = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private string _msgFormat;

		/// <summary>
		///     Gets or sets a value indicating whether this instance is records cache enabled.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is records cache enabled; otherwise, <c>false</c>.
		/// </value>
		[JsonDataMember("cacheEnabled")]
		public bool IsRecordsCacheEnabled { get { return _cacheEnabled; } set { _cacheEnabled = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private bool _cacheEnabled;

		/// <summary>
		///     Gets or sets the size of the records cache.
		/// </summary>
		/// <value>
		///     The size of the records cache.
		/// </value>
		[JsonDataMember("cacheSize")]
		public int RecordsCacheSize { get { return _cacheSize; } set { _cacheSize = value; } }

#if UNITY || UNITY3D
		[SerializeField]
#endif
		private int _cacheSize;
	}

	/// <summary>
	///     Log Record
	/// </summary>
	[JsonDataContract]
	public class LogRecord
	{
		public static DateTime? DefaultDateTime;

		/// <summary>
		///     Initializes the <see cref="LogRecord" /> class.
		/// </summary>
		static LogRecord()
		{
			DefaultDateTime = DateTime.UtcNow;
			JsonMapper.Default.RegisterImporter<string, LogRecordFormat>(
				input => (LogRecordFormat) Enum.Parse(typeof (LogRecordFormat), input));
			JsonMapper.Default.RegisterImporter<string, LogSourceType>(
				input => (LogSourceType) Enum.Parse(typeof (LogSourceType), input));
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="LogRecord" /> class.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="msgFormat">The MSG format.</param>
		/// <param name="args">The arguments.</param>
		public LogRecord(LogSourceType src, string msgFormat, params object[] args) : this()
		{
			Source = src;
			Message = string.Format(msgFormat, args);
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="LogRecord" /> class.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="msg">The MSG.</param>
		public LogRecord(LogSourceType src, string msg) : this()
		{
			Source = src;
			Message = msg;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="LogRecord" /> class.
		/// </summary>
		/// <param name="exc">The Exception.</param>
		public LogRecord(Exception exc) :
			this(LogSourceType.Exception, exc.ToString())
		{
			Exception = exc;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="LogRecord" /> class.
		/// </summary>
		public LogRecord()
		{
			if (DefaultDateTime.HasValue)
			{
				Time = DefaultDateTime.Value;
			}
		}

		/// <summary>
		///     Gets or sets the log record time.
		/// </summary>
		/// <value>
		///     The time.
		/// </value>
		[JsonDataMember("time")]
		public virtual DateTime Time { get; set; }

		/// <summary>
		///     Gets or sets the source.
		/// </summary>
		/// <value>
		///     The source.
		/// </value>
		[JsonDataMember("src")]
		public virtual LogSourceType Source { get; set; }

		/// <summary>
		///     Gets or sets the message.
		/// </summary>
		/// <value>
		///     The message.
		/// </value>
		[JsonDataMember("msg")]
		public virtual string Message { get; set; }

		/// <summary>
		///     Gets or sets the exception.
		/// </summary>
		/// <value>
		///     The exception.
		/// </value>
		[JsonDataMemberIgnore]
		public virtual Exception Exception { get; set; }

		/// <summary>
		///     Gets or sets the message format.
		/// </summary>
		/// <value>
		///     The message format.
		/// </value>
		[JsonDataMember("msgFormat")]
		public virtual LogRecordFormat MessageFormat { get; set; }

		/// <summary>
		///     Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///     A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("[{0}] {1}: {2}", Time, Source, Message);
		}
	}

	/// <summary>
	///     Log Record (generic)
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[JsonDataContract]
	public class LogRecord<T> : LogRecord
	{
		/// <summary>
		///     Gets or sets the data.
		/// </summary>
		/// <value>
		///     The data.
		/// </value>
		public virtual T Data { get; set; }
	}

	/// <summary>
	///     Interface for Loggers
	///     <remarks>(contains default loggers)</remarks>
	/// </summary>
	public interface ILoggers
	{
#if UNITY3D || UNITY_3D
		/// <summary>
		///     Gets the default console logger.
		///     <para />
		///     (wraps <see cref="UnityEngine.Debug" />)
		/// </summary>
		/// <value>
		///     The console logger.
		/// </value>
#else
	/// <summary>
	/// Gets the default console logger. <para/>		
	/// (wraps <see cref="System.Diagnostics.Debug"/>)
	/// </summary>
	/// <value>
	/// The console logger.
	/// </value>
#endif
		ILogger ConsoleLogger { get; }

		/// <summary>
		///     Gets the network logger.
		/// </summary>
		/// <value>
		///     The network logger.
		/// </value>
		ILogger NetworkLogger { get; }

		/// <summary>
		/// Gets the default file logger.
		/// </summary>
		/// <value>
		/// The file logger.
		/// </value>
		IFileLogger FileLogger { get; }
	}

	/// <summary>
	///     Default Loggers
	/// </summary>
	public sealed class Loggers :
#if UNITY_3D || UNITY3D
		MonoBehaviorBaseSingleton<Loggers>,
#else
		Singleton<ILoggers, Loggers>, 
#endif
		ILoggers
	{
		private ILogger _consoleLogger;

#if UNITY_3D || UNITY3D
		public LogSeverityType ConsoleLogSeverity;
#else
		public LogSeverityType ConsoleLogSeverity { get; set; }
#endif

		private ILogger _networkLogger;

#if UNITY_3D || UNITY3D
		public LogSeverityType NetworkLogSeverity;
#else
		public LogSeverityType NetworkLogSeverity { get; set; }
#endif

		/// <summary>
		///     Initializes the console logger.
		/// </summary>
		/// <param name="logger">The logger instance.</param>
		public void InitConsoleLogger(ILogger logger)
		{
			ArgumentValidator.AssertNotNull(logger, "logger");
			_consoleLogger = logger.Init();
			ConsoleLogSeverity = logger.Settings.Severity;
		}

		/// <summary>
		///     Initializes the network logger.
		/// </summary>
		/// <param name="logger">The logger.</param>
		public void InitNetworkLogger(ILogger logger)
		{
			ArgumentValidator.AssertNotNull(logger, "logger");
			_networkLogger = logger.Init();
			NetworkLogSeverity = logger.Settings.Severity;
		}

		/// <summary>
		/// Initializes the file logger.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="sid">The sid.</param>
		public void InitFileLogger(IFileLogger logger, string sid = null)
		{
			ArgumentValidator.AssertNotNull(logger, "logger");
			_fileLogger = logger.Init(sid);
			FileLogSeverity = logger.Settings.Severity;
		}

#if UNITY3D || UNITY_3D
		/// <summary>
		///     Gets the default console logger.
		///     <para />
		///     (wraps <see cref="UnityEngine.Debug" />)
		/// </summary>
		/// <value>
		///     The console logger.
		/// </value>
#else
	/// <summary>
	/// Gets the default console logger. <para/>		
	/// (wraps <see cref="System.Diagnostics.Debug"/>)
	/// </summary>
	/// <value>
	/// The console logger.
	/// </value>
#endif
		public ILogger ConsoleLogger
		{
			get
			{
				if (_consoleLogger == null)
				{
					return Locker.InitWithLock(ref _consoleLogger,
						() =>
						{
							var logger = new ConsoleLogger().Init(); // TODO init via IocManager
							ConsoleLogSeverity = logger.Settings.Severity; // TODO do wee need to override here?
							return logger;
						});
				}
				if (_consoleLogger != null)
				{
					_consoleLogger.Settings.Severity = ConsoleLogSeverity;
				}
				else
				{
#if UNITY || UNITY3D || UNITY_3D
					UnityEngine.Debug.LogWarning("ConsoleLogger is NULL.");
#else
					System.Diagnostics.Debug.WriteLine("ConsoleLogger is NULL.");
#endif
				}
				return _consoleLogger;
			}
		}

		/// <summary>
		///     Gets the network logger.
		/// </summary>
		/// <value>
		///     The network logger.
		/// </value>
		public ILogger NetworkLogger
		{
			get
			{
				if (_networkLogger == null)
				{
					return Locker.InitWithLock(ref _networkLogger,
						() =>
						{
							var logger = new ConsoleLogger().Init(); // TODO init via IocManager
							NetworkLogSeverity = logger.Settings.Severity; // TODO do wee need to override here?
							return logger;
						});
				}
				if (_networkLogger != null)
				{
					_networkLogger.Settings.Severity = NetworkLogSeverity;
				}
				else
				{
#if UNITY || UNITY3D || UNITY_3D
					UnityEngine.Debug.LogWarning("NetworkLogger is NULL.");
#else
					System.Diagnostics.Debug.WriteLine("ConsoleLogger is NULL.");
#endif
				}
				return _networkLogger;
			}
		}

		private IFileLogger _fileLogger;

#if UNITY_3D || UNITY3D
		public LogSeverityType FileLogSeverity;
#else
		public LogSeverityType FileLogSeverity { get; set; }
#endif

		/// <summary>
		///     Gets the default file logger.
		/// </summary>
		/// <value>
		///     The file logger.
		/// </value>
		public IFileLogger FileLogger
		{
			get
			{
				if (_fileLogger == null)
				{
					return Locker.InitWithLock(ref _fileLogger,
						() =>
						{
							var logger = new FileLogger().Init(Guid.NewGuid().ToString("N")); // TODO init via IocManager
							FileLogSeverity = logger.Settings.Severity; // TODO do wee need to override here?
							return logger;
						});
				}
				if (_fileLogger != null)
				{
					_fileLogger.Settings.Severity = FileLogSeverity;
				}
				else
				{
#if UNITY || UNITY3D || UNITY_3D
					UnityEngine.Debug.LogWarning("FileLogger is NULL.");
#else
					System.Diagnostics.Debug.WriteLine("FileLogger is NULL.");
#endif
				}
				return _fileLogger;
			}
		}
	}

#endregion

#region Enums

	/// <summary>
	///     Log Target Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum LogTargetType
	{
		/// <summary>
		///     Write to console
		/// </summary>
		Console,

		/// <summary>
		///     Write in to file
		/// </summary>
		File,

		/// <summary>
		///     Send to server
		/// </summary>
		Server
	}

	/// <summary>
	///     Log Source Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum LogSourceType
	{
		/// <summary>
		///     The trace
		/// </summary>
		Trace,

		/// <summary>
		///     The system information
		/// </summary>
		SysInfo,

		/// <summary>
		///     The error
		/// </summary>
		Error,

		/// <summary>
		///     The exception
		/// </summary>
		Exception,

		/// <summary>
		///     The warning
		/// </summary>
		Warning,

		/// <summary>
		///     The other
		/// </summary>
		Other
	}

	/// <summary>
	///     Log Record Format
	/// </summary>
	public enum LogRecordFormat
	{
		/// <summary>
		///     The text
		/// </summary>
		Text,

		/// <summary>
		///     The XML
		/// </summary>
		XML,

		/// <summary>
		///     The json
		/// </summary>
		JSON,

		/// <summary>
		///     The other
		/// </summary>
		Other
	}

	/// <summary>
	///     Log Severity Type
	/// </summary>
	public enum LogSeverityType
	{
		/// <summary>
		///     The critical
		/// </summary>
		Critical = 0,

		/// <summary>
		///     The normal
		/// </summary>
		Normal,

		/// <summary>
		///     The none
		/// </summary>
		None
	}

	/// <summary>
	///     File Backup Trigger Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum FileBackupTriggerType
	{
		/// <summary>
		///     None
		/// </summary>
		None,

		/// <summary>
		///     Maximum file size
		/// </summary>
		MaxFileSize,

		/// <summary>
		///     Maximum records count
		/// </summary>
		MaxRecordsCount,

		/// <summary>
		///     Time span
		/// </summary>
		TimeSpan
	}

	/// <summary>
	///     File Record Write Trigger Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum FileRecordWriteTriggerType
	{
		/// <summary>
		///     None
		/// </summary>
		None,

		/// <summary>
		///     Each record
		/// </summary>
		All,

		/// <summary>
		///     Each critical record
		/// </summary>
		Critical
	}

	/// <summary>
	///     File Send Trigger Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum FileSendTriggerType
	{
		/// <summary>
		///     The none
		/// </summary>
		None,

		/// <summary>
		///     The maximum file size
		/// </summary>
		MaxFileSize,

		/// <summary>
		///     The maximum records count
		/// </summary>
		MaxRecordsCount,

		/// <summary>
		///     The time span
		/// </summary>
		TimeSpan,

		/// <summary>
		///     The each record
		/// </summary>
		EachRecord
	}

#endregion
}