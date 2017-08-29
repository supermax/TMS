#define UNITY3D

using System;
using TMS.Common.Core;
using TMS.Common.Extensions;
using TMS.Common.Logging.Api;

namespace TMS.Common.Logging
{
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
}