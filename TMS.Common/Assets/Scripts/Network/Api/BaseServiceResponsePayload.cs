using System;
using TMS.Common.Extensions;
using TMS.Common.Network.Request.Api;
using TMS.Common.Network.Response.Api;

namespace TMS.Common.Network.Api
{
	public abstract class BaseServiceResponsePayload : BaseServicePayload
	{
		protected internal BaseServiceResponsePayload(BaseServiceRequestPayload parentRequestPayload,
			ServiceResponseState state, Exception error = null)
			: base(parentRequestPayload.ServiceName)
		{
			ArgumentValidator.AssertNotNull(parentRequestPayload, "parentRequestPayload");

			ParentRequestPayload = parentRequestPayload;
			Tag = parentRequestPayload.Tag;
			IsCustomService = parentRequestPayload.IsCustomService;

			ResponseState = state;
			ResponseError = error;
		}

		public virtual ServiceResponseState ResponseState { get; set; }

		public virtual BaseServiceRequestPayload ParentRequestPayload { get; set; }

		public virtual string ResponseString { get; set; }

		public virtual Exception ResponseError { get; set; }
	}

	public abstract class BaseServiceResponsePayload<T> : BaseServiceResponsePayload
	{
		protected BaseServiceResponsePayload(BaseServiceRequestPayload parentRequestPayload, T data,
			ServiceResponseState state, Exception error = null)
			: base(parentRequestPayload, state, error)
		{
			Response = data;
		}

		public virtual T Response { get; set; }
	}

	// TODO
	public abstract class BaseServiceResponsePayload<TData, TParent> : BaseServiceResponsePayload<TData>
		where TParent : BaseServiceRequestPayload
	{
		protected BaseServiceResponsePayload(TParent parentRequestPayload, TData data,
			ServiceResponseState state, Exception error = null)
			: base(parentRequestPayload, data, state, error)
		{
			
		}

		public new virtual TParent ParentRequestPayload { get; set; }
	}
}