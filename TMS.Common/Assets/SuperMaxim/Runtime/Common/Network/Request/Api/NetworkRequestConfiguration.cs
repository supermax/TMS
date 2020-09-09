#region Usings

using TMS.Common.Helpers;
using TMS.Common.Network.Api;
using TMS.Common.Network.Config.Api;
using TMS.Common.Network.Response.Api;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

#endregion

namespace TMS.Common.Network.Request.Api
{
	[JsonDataContract]
	public class NetworkRequestConfiguration : BaseServiceConfiguration
	{
		static NetworkRequestConfiguration()
		{
			JsonMapper.Default.RegisterImporter<string, ServiceRequestType>(EnumHelper.Convert<ServiceRequestType>);
			JsonMapper.Default.RegisterImporter<string, ServiceRequestCacheType>(EnumHelper.Convert<ServiceRequestCacheType>);
			JsonMapper.Default.RegisterImporter<string, ServiceRequestCacheResendTrigger>(EnumHelper.Convert<ServiceRequestCacheResendTrigger>);
			JsonMapper.Default.RegisterImporter<string, ResponsePayloadDataType>(EnumHelper.Convert<ResponsePayloadDataType>);
		}

		[JsonDataMember("url")]
		public string ServiceUrl { get; set; }

		[JsonDataMember("timeout")]
		public float RequestTimeout { get; set; }

		[JsonDataMember("maxRetries")]
		public int RequestMaxRetriesNum { get; set; }

		[JsonDataMember("method")]
		public ServiceRequestType RequestType { get; set; }

		[JsonDataMember("cacheMode")] // TODO
		public ServiceRequestCacheType CacheType { get; set; }

		[JsonDataMember("maxCacheSize")] // TODO
		public int MaxCacheSize { get; set; }

		[JsonDataMember("cacheTrigger")] // TODO
		public ServiceRequestCacheResendTrigger CacheResendTrigger { get; set; }

		[JsonDataMember("suppressAll")] // TODO
		public bool SuppressAllErrors { get; set; }

		[JsonDataMember("suppressErrors")] // TODO
		public int[] SuppressErrors { get; set; }

		[JsonDataMember("responseType")]
		public ResponsePayloadDataType ResponseDataType { get; set; }

		[JsonDataMember("urlRandomParam")]
		public bool RandomUrlParam { get; set; }

		public NetworkRequestConfiguration Clone()
		{
			var clone = new NetworkRequestConfiguration();
			DataMapper.Default.CopyPropertyValues(this, clone);
			return clone;
		}
	}
}