using System;

namespace TMS.Common.Network.Request.Api
{
	[Flags]
	public enum ServiceRequestCacheType
	{
		None = 0,
		OnlineCache,
		OfflineCache
	}
}