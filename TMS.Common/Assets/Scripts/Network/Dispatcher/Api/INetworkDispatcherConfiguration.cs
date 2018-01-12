#region

using TMS.Common.Config;
using TMS.Common.Network.Request.Api;

#endregion

namespace TMS.Common.Network.Dispatcher.Api
{
	public interface INetworkDispatcherConfiguration : IBaseConfiguration
	{
		NetworkRequestConfigurationData RequestsConfig { get; }		
	}
}