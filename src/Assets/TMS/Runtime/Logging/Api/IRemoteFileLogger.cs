namespace TMS.Common.Logging.Api
{
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
}