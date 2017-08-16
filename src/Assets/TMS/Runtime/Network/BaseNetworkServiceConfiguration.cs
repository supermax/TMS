#region

using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Network
{
	[JsonDataContract]
	public abstract class BaseNetworkServiceConfiguration
	{
		[JsonDataMember("name")]
		public virtual string ServiceName { get; set; }
	}
}