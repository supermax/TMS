#define UNITY3D

using System;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;
using UnityEngine;

namespace TMS.Common.Logging.Api
{
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
}