using System;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

namespace TMS.Common.Logging.Api
{
	/// <summary>
	///     Remote File Logger Settings
	/// </summary>
	[JsonDataContract]
	[Serializable]
	public class RemoteFileLoggerSettings : FileLoggerSettings
	{
		/// <summary>
		///     Gets or sets the send trigger.
		/// </summary>
		/// <value>
		///     The send trigger.
		/// </value>
		[JsonDataMember("sndTrigger")]
		public FileSendTriggerType SendTrigger { get; set; }
	}
}