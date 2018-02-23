using System;

namespace TMS.Common.Logging.Api
{
	/// <summary>
	///     File Record Write Trigger Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum FileRecordWriteTriggerType
	{
		/// <summary>
		///     None
		/// </summary>
		None,

		/// <summary>
		///     Each record
		/// </summary>
		All,

		/// <summary>
		///     Each critical record
		/// </summary>
		Critical
	}
}