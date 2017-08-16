#region Usings

using System;
using System.IO;
using TMS.Common.Core;
using TMS.Common.Extensions;
using TMS.Common.Serialization.Json;
using UnityEngine;

#endregion

namespace TMS.Common.Logging
{
	/// <summary>
	///     Unity Console Logger
	/// </summary>
	public class ConsoleLogger : BaseLogger, IConsoleLogger
	{
		protected const string DefaultSettingsFilePath = @"Config/ConsoleLoggerConfig";

		/// <summary>
		///     Initializes a new instance of the <see cref="ConsoleLogger" /> class.
		/// </summary>
		public ConsoleLogger()
		{
			SettingsFilePath = DefaultSettingsFilePath;
		}

		/// <summary>
		///     Writes the msg.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <param name="src">The source.</param>
		protected override void WriteInternal(string msg, LogSourceType src)
		{
			if (Settings.Severity == LogSeverityType.None) return;

			ArgumentValidator.AssertNotNull(msg, "msg");
		    Action act;
            switch (src)
			{
				case LogSourceType.Warning:
			        act = () => Debug.LogWarning(msg);
					break;

				case LogSourceType.Error:
					act = () => Debug.LogError(msg);
					break;

				default:
					if (Settings.Severity == LogSeverityType.Critical) return;
					act = () => Debug.Log(msg);
					break;
			}
            DispatcherProxy.Default.Invoke(act, DispatcherPriority.Normal);
        }

		/// <summary>
		///     Writes the exc.
		/// </summary>
		/// <param name="exc">The exception.</param>
		protected override void WriteInternal(Exception exc)
		{
			if (Settings.Severity == LogSeverityType.None) return;

			ArgumentValidator.AssertNotNull(exc, "exc");
			Action act = () => Debug.LogException(exc);
            DispatcherProxy.Default.Invoke(act, DispatcherPriority.Normal);
        }

		/// <summary>
		///     Writes the record.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <param name="src">The source.</param>
		protected override void WriteInternal(LogRecord record, LogSourceType src)
		{
			if (Settings.Severity == LogSeverityType.None) return;

			ArgumentValidator.AssertNotNull(record, "record");
			WriteInternal(record.ToString(), src);
		}

		/// <summary>
		/// Clears the log.
		/// </summary>
		public override ILogger Clear()
		{
            Action act = Debug.ClearDeveloperConsole;
            DispatcherProxy.Default.Invoke(act, DispatcherPriority.Normal);
            return this;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <returns>
		/// Instance of initialized logger
		/// </returns>
		protected override ILogger InitInternal()
		{
		    Action act;
		    try
		    {
		        if (!File.Exists(SettingsFilePath))
		        {
		            act = () => Debug.LogWarningFormat("Cannot load logger settings from file path: \"{0}\".", SettingsFilePath);
		        }
		        else
		        {
		            act = () =>
		            {
		                var json = Resources.Load<TextAsset>(SettingsFilePath).text;
		                Settings = JsonMapper.Default.ToObject<LoggerSettings>(json);
		            };
		        }
		    }
		    catch (Exception ex)
		    {
		        act = () => Debug.LogException(ex);
		    }
		    DispatcherProxy.Default.Invoke(act, DispatcherPriority.Normal);
            return this;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <returns></returns>
		public new IConsoleLogger Init()
		{
			var instance = base.Init() as IConsoleLogger;
			return instance;
		}
	}	
}