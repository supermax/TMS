using System;
using TMS.Common.Network.Request.Api;
using TMS.Common.Network.Response.Api;

namespace TMS.Common.Network.Api
{
	public interface IServiceFactory
	{
		//IServiceRequestTask CreateTask(IServiceRequestPayload payload);

		IServiceRequestTask CreateTask<T>(IServiceRequestPayload payload);

		IServiceHandler<TReq, TRes> CreateServiceHandler<TReq, TRes>(
			string serviceName,
			ResponsePayloadDataType responsePayloadDataType);

		IServiceRequestPayload<T> CreateRequestPayload<T>(
			string serviceName, 
			T data, 
			bool isCustomService,
			string serviceUrl = null);

		IServiceResponsePayload<TRes> CreateResponsePayload<TReq, TRes>(
			IServiceRequestPayload<TReq> parent,
			TRes data,
			ServiceResponseState state,
			Exception error = null);
	}
}