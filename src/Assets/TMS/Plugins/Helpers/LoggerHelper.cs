#region Usings

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

#endregion

namespace TMS.Common.Helpers
{
	/// <summary>
	///     Logger Helper
	/// </summary>
	public static class LoggerHelper
	{
		private const long MaxFileLength = 500000;
		private static readonly string EventLogSrcName = "TMS";

		/// <summary>
		///     Initializes the <see cref="LoggerHelper" /> class.
		/// </summary>
		static LoggerHelper()
		{
			if (Application.isEditor) return;
			EventLogSrcName = Process.GetCurrentProcess().ProcessName;
			CreateLoggers();
			AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
		}

		/// <summary>
		///     Called when [current domain unhandled exception].
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">
		///     The <see cref="System.UnhandledExceptionEventArgs" /> instance containing the event data.
		/// </param>
		private static void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var exc = e.ExceptionObject as Exception ?? (e.ExceptionObject != null
				                                             ? new ApplicationException(e.ExceptionObject.ToString())
				                                             : new ApplicationException("Unknown Application Exception"));
			HandleException(exc);
		}

		/// <summary>
		///     Creates the logger.
		/// </summary>
		private static void CreateTraceLog()
		{
			try
			{
				//var ass = Assembly.GetExecutingAssembly().Location;
				var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}
				folderPath = Path.Combine(folderPath, EventLogSrcName);
				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}
				folderPath = Path.Combine(folderPath, "Logs");
				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}

				var filePath = Path.Combine(folderPath, string.Format("{0}_{1}.log", "TraceLog", DateTime.Now.ToFileTime()));
				var textTrace = new TextWriterTraceListener(new StreamWriter(filePath, false, Encoding.UTF8)
					{
						AutoFlush = true,
					});
				Trace.Listeners.Add(textTrace);
			}
			catch (Exception exc)
			{
				Trace.WriteLine(exc, EventLogId.AppExc.ToString());
			}
		}

		/// <summary>
		///     Creates the loggers.
		/// </summary>
		private static void CreateLoggers()
		{
			CreateTraceLog();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is during startup.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is during startup; otherwise, <c>false</c>.
		/// </value>
		public static bool IsDuringStartup { get; set; }

		/// <summary>
		///     Handles the exception.
		/// </summary>
		/// <param name="appExc">The app exc.</param>
		/// <param name="showMsgBox">
		///     if set to <c>true</c> [show MSG box].
		/// </param>
		public static void HandleException(Exception appExc, bool showMsgBox = true)
		{
			try
			{
				if (showMsgBox)
				{
					var ex = appExc;
					var message = ex.Message;

					while (ex.InnerException != null)
					{
						ex = ex.InnerException;
						message = string.Format("{0}\r\n---  ---  ---\r\n{1}", ex.Message, message);
					}

					if (message.Length > 250)
					{
						message = message.Substring(0, 250);
					}

					var proc = Process.GetCurrentProcess();
					Trace.WriteLine(string.Format("[{0}]: {1}", proc.ProcessName, message));
				}
			}
			catch (Exception exc)
			{
				Trace.WriteLine(exc, EventLogId.AppExc.ToString());
			}
			finally
			{
				var str = string.Format("###\r\nException Occurred at: {0}\r\nDetails{1}\r\n###", DateTime.Now, appExc);
				Trace.WriteLine(str, EventLogId.AppExc.ToString());
			}
		}

		/// <summary>
		///     Writes the start process log.
		/// </summary>
		public static void WriteStartProcessLog()
		{
			var entry = String.Format("{0} Started at {1}", EventLogSrcName, DateTime.Now);
			Trace.WriteLine(entry, EventLogId.ProcessLog.ToString());
		}

		/// <summary>
		///     Writes the stop process log.
		/// </summary>
		public static void WriteStopProcessLog()
		{
			var entry = string.Format("{0} Stopped at {1}", EventLogSrcName, DateTime.Now);
			Trace.WriteLine(entry, EventLogId.ProcessLog.ToString());
		}

		#region Nested type: EventLogId

		/// <summary>
		/// </summary>
		internal enum EventLogId
		{
			/// <summary>
			/// </summary>
			AppExc = 0,

			/// <summary>
			/// </summary>
			ProcessLog = 1,

			/// <summary>
			/// </summary>
			LogError = 2,
		}

		#endregion
	}
}