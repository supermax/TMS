using System;
using TMS.Common.Extensions;
using TMS.Common.Network.Api;
using TMS.Common.Network.Request.Api;

namespace TMS.Common.Network.Response.Api
{
	public abstract class BaseServiceResponsePayload : BaseServicePayload, IServiceResponsePayload
	{
		protected internal BaseServiceResponsePayload(IServiceRequestPayload parentRequestPayload,
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

		public virtual IServiceRequestPayload ParentRequestPayload { get; set; }

		public virtual string ResponseString { get; set; }

		public virtual Exception ResponseError { get; set; }
	}
}