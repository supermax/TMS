using System;

namespace TMS.Common.Logging.Api
{
	/// <summary>
	///     Log Source Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum LogSourceType
	{
		/// <summary>
		///     The trace
		/// </summary>
		Trace,

		/// <summary>
		///     The system information
		/// </summary>
		SysInfo,

		/// <summary>
		///     The error
		/// </summary>
		Error,

		/// <summary>
		///     The exception
		/// </summary>
		Exception,

		/// <summary>
		///     The warning
		/// </summary>
		Warning,

		/// <summary>
		///     The other
		/// </summary>
		Other
	}
}