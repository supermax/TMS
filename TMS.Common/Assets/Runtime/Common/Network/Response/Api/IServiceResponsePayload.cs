using System;
using TMS.Common.Network.Request.Api;

namespace TMS.Common.Network.Response.Api
{
	public interface IServiceResponsePayload
	{
		ServiceResponseState ResponseState { get; set; }

		IServiceRequestPayload ParentRequestPayload { get; set; }

		string ResponseString { get; set; }

		Exception ResponseError { get; set; }
	}
}