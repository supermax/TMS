using TMS.Common.Extensions;

namespace TMS.Common.Network
{
	public abstract class BaseServiceResponsePayload : BaseServicePayload
	{
		protected internal BaseServiceResponsePayload(BaseServiceRequestPayload parentRequestPayload,
			ServiceResponseState state, string error = null)
			: base(parentRequestPayload.ServiceName)
		{
			ArgumentValidator.AssertNotNull(parentRequestPayload, "parentRequestPayload");

			ParentRequestPayload = parentRequestPayload;
			IsCustomService = parentRequestPayload.IsCustomService;

			ResponseState = state;
			ResponseError = error;
		}

		public virtual ServiceResponseState ResponseState { get; set; }
		public virtual BaseServiceRequestPayload ParentRequestPayload { get; set; }
		public virtual string ResponseString { get; set; }
		public virtual string ResponseError { get; set; }
	}

	public abstract class BaseServiceResponsePayload<T> : BaseServiceResponsePayload
	{
		protected BaseServiceResponsePayload(BaseServiceRequestPayload parentRequestPayload, T data,
			ServiceResponseState state, string error = null)
			: base(parentRequestPayload, state, error)
		{
			ResponseData = data;
		}

		public virtual T ResponseData { get; set; }
	}

	// TODO
	public abstract class BaseServiceResponsePayload<TData, TParent> : BaseServiceResponsePayload<TData>
		where TParent : BaseServiceRequestPayload
	{
		protected BaseServiceResponsePayload(TParent parentRequestPayload, TData data,
			ServiceResponseState state, string error = null)
			: base(parentRequestPayload, data, state, error)
		{
			
		}

		public virtual new TParent ParentRequestPayload { get; set; }
	}

	public enum ServiceResponseState
	{
		Unknown = 0,

		Success,

		ServerError,

		ClientError,

		AbortedByClient,

		AbortedByServer,

		ConnectionTimeout, // TODO

		InternetReachabilityError, // TODO
	}
}