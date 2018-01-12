#region Usings

using System;
using TMS.Common.Network.Request.Api;
using TMS.Common.Network.Response.Api;

#endregion

namespace TMS.Common.Network.Api
{
	public interface IServiceHandler<TRequestData, TResponseData> : IDisposable
	{
		bool IsEnabled { get; set; }

		string ServiceName { get; }

		void AbortRequest();

		void AbortAllRequests();

		IServiceRequestPayload<TRequestData> SendRequest(
			TRequestData data,
			Action<IServiceResponsePayload<TResponseData>> responseCallback = null,
			Predicate<IServiceResponsePayload<TResponseData>> responseCallbackPredicate = null,
			object tag = null);

		IServiceRequestPayload<TRequestData> SendRequest(
			string customServiceUrl, TRequestData data,
			Action<IServiceResponsePayload<TResponseData>> responseCallback = null,
			Predicate<IServiceResponsePayload<TResponseData>> responseCallbackPredicate = null,
			object tag = null);
	}
}