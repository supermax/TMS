#region

using System;

#endregion

namespace TMS.Common.Network
{
	public interface IServiceRequestTask : IDisposable
	{
		BaseServiceRequestPayload RequestPayload { get; }
		void Start();
		void Stop();
	}
}