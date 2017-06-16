#region

using System.Collections.Generic;
using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Network
{
	[JsonDataContract]
	public class NetworkRequestConfigurationData
	{
		[JsonDataMember("default")]
		public NetworkRequestConfiguration Default { get; set; }

		[JsonDataMember("services")]
		public Dictionary<string, NetworkRequestConfiguration> Services { get; set; }		
	}
}