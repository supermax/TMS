#region

using System;
using TMS.Common.Core;

#endregion

namespace TMS.Common.Tasks.Timeout
{
	public class TimeoutDelegatePayload : DelegatePayloadBase
	{
		public TimeoutDelegatePayload(Delegate onTimeoutMethod, TimeSpan timeout, bool isRepeatative = false, params object[] onTimeoutMethodArgs)
			: base(onTimeoutMethod, onTimeoutMethodArgs)
		{
			Timeout = timeout;
			IsRepeatative = isRepeatative;
		}

		internal virtual TimeSpan Timeout { get; set; }

		internal DateTime TimeoutStartTime { get; set; }

		public virtual bool IsTimeOut { get; protected internal set; }

		public virtual bool IsRepeatative { get; protected internal set; }

		internal bool IsTimeoutOccured()
		{
			if (IsTimeOut) return true;
			var span = DateTime.Now - TimeoutStartTime;
			IsTimeOut = span >= Timeout;
			return IsTimeOut;
		}
	}
}