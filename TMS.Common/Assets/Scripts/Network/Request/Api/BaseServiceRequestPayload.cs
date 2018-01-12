#region Usings

using System;
using TMS.Common.Serialization.Json;
using System.Collections.Generic;
using TMS.Common.Network.Api;

#endregion

namespace TMS.Common.Network.Request.Api
{
	[Serializable]
	[JsonDataContract]
	public abstract class BaseServiceRequestPayload : BaseServicePayload, IServiceRequestPayload
	{
		protected BaseServiceRequestPayload(string serviceName) : base(serviceName)
		{
			
		}
		
		[JsonDataMember("url")]
		public virtual string ServiceUrl { get; set; }

		[JsonDataMember("customUrl")]
		public virtual string CustomServiceUrl { get; set; }

		[JsonDataMember("urlParams")]
		public virtual Dictionary<string, string> UrlParams { get; set; }

		[JsonDataMember("postParams")]
		public virtual Dictionary<string, string> PostParams { get; set; }

		[JsonDataMember("headerParams")]
		public virtual Dictionary<string, string> HeaderParams { get; set; }

		[JsonDataMember("rawData")]
		public virtual byte[] RawRequestData { get; set; }

		[JsonDataMember("config")]
		public virtual NetworkRequestConfiguration Config { get; set; }

		[JsonDataMemberIgnore]
		public virtual bool Abort { get; set; }

		[JsonDataMemberIgnore]
		public virtual bool AbortAll { get; set; }

		[JsonDataMemberIgnore]
		public string ResponseCallbackId { get; set; }
	}
}