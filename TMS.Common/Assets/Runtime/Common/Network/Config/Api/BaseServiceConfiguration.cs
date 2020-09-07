#region

using System;
using System.Collections.Generic;
using TMS.Common.Config;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

#endregion

namespace TMS.Common.Network.Config.Api
{
	[Serializable]
	[JsonDataContract]
	public abstract class BaseServiceConfiguration : BaseConfiguration
	{
		[JsonDataMember("name")]
		public virtual string ServiceName { get; set; }
	}
}