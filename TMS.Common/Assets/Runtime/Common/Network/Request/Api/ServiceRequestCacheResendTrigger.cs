using System;

namespace TMS.Common.Network.Request.Api
{
	[Flags]
	public enum ServiceRequestCacheResendTrigger
	{
		None = 0,
		MaxCacheSize,
		SericeRecovery
	}
}