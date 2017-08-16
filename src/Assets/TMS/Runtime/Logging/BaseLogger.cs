#region Usings

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace TMS.Common.Logging
{
	/// <summary>
	///     Base Logger
	/// </summary>
	public abstract class BaseLogger : ILogger
	{
		#region Implementation of ILogger

		/// <summary>
		///     Gets or sets the settings.
		/// </summary>
		/// <value>
		///     The settings.
		/// </value>
		public virtual LoggerSettings Settings { get; set; }

		/// <summary>
		///     Gets or sets the settings file path.
		/// </summary>
		/// <value>
		///     The settings file path.
		/// </value>
		public virtual string SettingsFilePath { get; set; }

		/// <summary>
		///     Gets or sets the records.
		/// </summary>
		/// <value>
		///     The records.
		/// </value>
		public virtual List<LogRecord> Records { get; set; }

		/// <summary>
		/// Writes the specified source.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public virtual ILogger Write(LogSourceType src, Delegate method)
	    {
	        var logger =  Write(src, "{0}.{1}()", method.Target, method.Method.Name);
	        return logger;
	    }

	    /// <summary>
		///     Writes the specified record.
		/// </summary>
		/// <param name="record">The record.</param>
		public virtual ILogger Write(LogRecord record)
		{
			switch (Settings.Severity)
			{
				case LogSeverityType.Critical:
					switch (record.Source)
					{
						case LogSourceType.Error:
							WriteInternal(record, record.Source);
							break;

						case LogSourceType.Exception:
							WriteInternal(record.Exception);
							break;
					}
					break;

				case LogSeverityType.Normal:
					switch (record.Source)
					{
						case LogSourceType.Exception:
							WriteInternal(record.Exception);
							break;

						default:
							WriteInternal(record, record.Source);
							break;
					}
					break;
			}

			if (!Settings.IsRecordsCacheEnabled) return this;
			AddRecord(record);
			return this;
		}

		/// <summary>
		///     Writes the msg.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <param name="src">The source.</param>
		protected virtual void WriteInternal(string msg, LogSourceType src) { }

		/// <summary>
		///     Writes the exc.
		/// </summary>
		/// <param name="exc">The exception.</param>
		protected virtual void WriteInternal(Exception exc) { }

		/// <summary>
		///     Writes the record.
		/// </summary>
		/// <param name="record">The record.</param>
		/// <param name="src">The source.</param>
		protected virtual void WriteInternal(LogRecord record, LogSourceType src) { }

		/// <summary>
		///     Adds the record.
		/// </summary>
		/// <param name="record">The record.</param>
		protected virtual void AddRecord(LogRecord record)
		{
			if (Records == null)
			{
				Records = new List<LogRecord>();
			}
			else if (Records.Count > Settings.RecordsCacheSize)
			{
				Clear();
			}

			Records.Add(record);
			OnNewRecordAdded(record);
		}

		/// <summary>
		/// Called when [new record added].
		/// </summary>
		/// <param name="record">The record.</param>
		protected virtual void OnNewRecordAdded(LogRecord record) { }

		/// <summary>
		/// Called when [records cleared].
		/// </summary>
		/// <param name="clearedRecords">The cleared records.</param>
		protected virtual void OnRecordsCleared(IEnumerable<LogRecord> clearedRecords) { }

		/// <summary>
		///     Writes the specified record.
		/// </summary>
		/// <param name="src">The source type.</param>
		/// <param name="msgFormat">The MSG format.</param>
		/// <param name="args">The arguments.</param>
		public virtual ILogger Write(LogSourceType src, string msgFormat, params object[] args)
		{
			var msg = string.Format(msgFormat, args);
			return Write(src, msg);
		}

		/// <summary>
		///     Writes the specified records.
		/// </summary>
		/// <param name="records">The records.</param>
		public virtual ILogger Write(IEnumerable<LogRecord> records)
		{
			foreach (var record in records)
			{
				Write(record);
			}
			return this;
		}

		/// <summary>
		///     Writes the specified source.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="msg">The MSG.</param>
		public virtual ILogger Write(LogSourceType src, string msg)
		{
			msg = string.Format(Settings.DefaultMessageFormat, msg);
			var rec = new LogRecord(src, msg);
			return Write(rec);
		}

		/// <summary>
		/// Writes the specified MSG.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <returns></returns>
		public virtual ILogger Write(string msg)
		{
			return Write(LogSourceType.Trace, msg);
		}

		/// <summary>
		/// Writes the specified MSG with given format and args.
		/// </summary>
		/// <param name="msgFormat">The MSG format.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public virtual ILogger Write(string msgFormat, params object[] args)
		{
			return Write(LogSourceType.Trace, msgFormat, args);
		}

		/// <summary>
		///     Writes the specified exception.
		/// </summary>
		/// <param name="exc">The exception.</param>
		public virtual ILogger Write(Exception exc)
		{
			var rec = new LogRecord(exc);
			return Write(rec);
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <returns>
		/// Instance of initialized logger
		/// </returns>
		public virtual ILogger Init()
		{
			InitInternal();

			if (Settings != null) return this; 
			Settings = new LoggerSettings
			{
				IsAutoTimeStampEnabled = true,
				LogTarget = LogTargetType.Console,
				TimeStampFormat = "T",
				DefaultMessageFormat = "{0}"
			};
			return this;
		}

		/// <summary>
		/// Clears the log.
		/// </summary>
		public virtual ILogger Clear()
		{
			var prevRecords = Records.ToArray();
			Records.Clear();
			OnRecordsCleared(prevRecords);
			return this;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		/// <returns>
		/// Instance of initialized logger
		/// </returns>
		protected virtual ILogger InitInternal()
		{
			return this;
		}

		public virtual void Dispose()
		{
			// TODO implement dispose pattern
		}

		#endregion
	}
}