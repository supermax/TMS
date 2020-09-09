#define UNITY3D

namespace TMS.Common.Logging.Api
{
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
}