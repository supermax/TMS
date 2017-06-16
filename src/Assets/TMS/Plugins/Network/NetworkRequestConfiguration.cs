#region

using System;
using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Network
{
	[JsonDataContract]
	public class NetworkRequestConfiguration : BaseNetworkServiceConfiguration
	{
		[JsonDataMember("url")]
		public virtual string ServiceUrl { get; set; }

		[JsonDataMember("timeout")]
		public float RequestTimeout { get; set; }

		[JsonDataMember("max_retries")]
		public int RequestMaxRetriesNum { get; set; }

		[JsonDataMember("ps")]
		public bool PostSecret { get; set; }

		[JsonDataMember("type")]
		public int ServiceType { get; set; }

		[JsonDataMember("cacheMode")] // TODO
		public ServiceRequestCacheType CacheType { get; set; }

		[JsonDataMember("maxCacheSize")]
		public int MaxCacheSize { get; set; }

		[JsonDataMember("cacheTrigger")]
		public ServiceRequestCacheResendTrigger CacheResendTrigger { get; set; }
	}

	[Flags]
	public enum ServiceRequestCacheType
	{
		None = 0,
		OnlineCache,
		OfflineCache
	}

	[Flags]
	public enum ServiceRequestCacheResendTrigger
	{
		None = 0,
		MaxCacheSize,
		SericeRecovery
	}
}