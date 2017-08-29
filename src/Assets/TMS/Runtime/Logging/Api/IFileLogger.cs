using System;

namespace TMS.Common.Logging.Api
{
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

		/// <summary>
		/// Initializes the specified sid.
		/// </summary>
		/// <param name="sid">The sid.</param>
		/// <param name="settings">The settings.</param>
		/// <returns></returns>
		IFileLogger Init(string sid, FileLoggerSettings settings = null);

		/// <summary>
		///     Saves to file.
		/// </summary>
		void SaveToFile(EventHandler<FileLoggerCallbackArgs> callback);

		/// <summary>
		///     Loads from file.
		/// </summary>
		void LoadFromFile(EventHandler<FileLoggerCallbackArgs> callback);

		/// <summary>
		/// Loads from file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="callback">The callback.</param>
		void LoadFromFile(string filePath, EventHandler<FileLoggerCallbackArgs> callback);

		/// <summary>
		///     Saves to file.
		/// </summary>
		void SaveToFile(string text, EventHandler<FileLoggerCallbackArgs> callback);

		/// <summary>
		/// Flushes this instance.
		/// </summary>
		void Flush();

		/// <summary>
		/// Gets the saved log file paths.
		/// </summary>
		/// <returns></returns>
		string[] GetSavedLogFilePaths();
	}
}