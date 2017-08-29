#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TMS.Common.Logging.Api
{
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

		/// <summary>
		/// Writes the specified source.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
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
}