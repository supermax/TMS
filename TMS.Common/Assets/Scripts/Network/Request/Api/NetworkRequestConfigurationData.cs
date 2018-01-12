#region

using System.Collections.Generic;
using TMS.Common.Config;
using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Network.Request.Api
{
	[JsonDataContract]
	public class NetworkRequestConfigurationData : BaseConfiguration
	{
		[JsonDataMember("default")]
		public NetworkRequestConfiguration Default { get; set; }

		[JsonDataMember("services")]
		public Dictionary<string, NetworkRequestConfiguration> Services { get; set; }		
	}
}