using System;

namespace TMS.Common.Logging.Api
{
	/// <summary>
	///     Log Target Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum LogTargetType
	{
		/// <summary>
		///     Write to console
		/// </summary>
		Console,

		/// <summary>
		///     Write in to file
		/// </summary>
		File,

		/// <summary>
		///     Send to server
		/// </summary>
		Server
	}
}