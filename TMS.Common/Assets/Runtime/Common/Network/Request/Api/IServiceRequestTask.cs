#region

using System;

#endregion

namespace TMS.Common.Network.Request.Api
{
	public interface IServiceRequestTask : IDisposable
	{
		void Start();

		void Stop();
	}
}