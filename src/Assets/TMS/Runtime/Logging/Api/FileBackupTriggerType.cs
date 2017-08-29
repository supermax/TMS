using System;

namespace TMS.Common.Logging.Api
{
	/// <summary>
	///     File Backup Trigger Type
	///     <remarks> » defined with <see cref="FlagsAttribute" /></remarks>
	/// </summary>
	[Flags]
	public enum FileBackupTriggerType
	{
		/// <summary>
		///     None
		/// </summary>
		None,

		/// <summary>
		///     Maximum file size
		/// </summary>
		MaxFileSize,

		/// <summary>
		///     Maximum records count
		/// </summary>
		MaxRecordsCount,

		/// <summary>
		///     Time span
		/// </summary>
		TimeSpan
	}
}