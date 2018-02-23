#region

using System;
using System.Collections.Generic;
using TMS.Common.Config;
using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Network.Api
{
	[Serializable]
	[JsonDataContract]
	public abstract class BaseNetworkServiceConfiguration : BaseConfiguration
	{
		[JsonDataMember("name")]
		public virtual string ServiceName { get; set; }
	}
}