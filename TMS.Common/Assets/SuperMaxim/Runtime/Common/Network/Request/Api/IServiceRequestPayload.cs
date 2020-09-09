using System.Collections.Generic;
using TMS.Common.Network.Api;

namespace TMS.Common.Network.Request.Api
{
	public interface IServiceRequestPayload : IServicePayload
	{
		string ServiceUrl { get; set; }

		string CustomServiceUrl { get; set; }

		Dictionary<string, string> UrlParams { get; set; }

		Dictionary<string, string> PostParams { get; set; }

		Dictionary<string, string> HeaderParams { get; set; }

		bool Abort { get; set; }

		bool AbortAll { get; set; }

		string ResponseCallbackId { get; set; }

		byte[] RawRequestData { get; set; }

		NetworkRequestConfiguration Config { get; set; }
	}	
}