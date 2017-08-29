using System;

namespace TMS.Common.Logging.Api
{
	/// <summary>
	///     File Send Trigger Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum FileSendTriggerType
	{
		/// <summary>
		///     The none
		/// </summary>
		None,

		/// <summary>
		///     The maximum file size
		/// </summary>
		MaxFileSize,

		/// <summary>
		///     The maximum records count
		/// </summary>
		MaxRecordsCount,

		/// <summary>
		///     The time span
		/// </summary>
		TimeSpan,

		/// <summary>
		///     The each record
		/// </summary>
		EachRecord
	}
}