#region

using System;
using TMS.Common.Core;
using TMS.Common.Extensions;

#endregion

namespace TMS.Common.Tasks.Timeout
{
	public class TimeoutDelegateCancelPayload : DelegatePayloadBase
	{
		public TimeoutDelegateCancelPayload(TimeoutDelegatePayload timeoutPayload,
			Delegate onCancelTimeoutMethod = null, params object[] onCancelTimeoutMethodArgs)
			: base(onCancelTimeoutMethod, onCancelTimeoutMethodArgs)
		{
			ArgumentValidator.AssertNotNull(timeoutPayload, "timeoutPayload");
			
			TimeoutPayload = timeoutPayload;
		}

		internal TimeoutDelegatePayload TimeoutPayload { get; private set; }

		public string CancelationReason { get; set; }

		public override void Dispose()
		{
			TimeoutPayload.Dispose();
			base.Dispose();
		}
	}
}