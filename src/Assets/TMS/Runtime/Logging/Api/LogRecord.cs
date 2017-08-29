#define UNITY3D

using System;
using TMS.Common.Serialization.Json;

namespace TMS.Common.Logging.Api
{
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
}