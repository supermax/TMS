#region

using TMS.Common.Config;

#endregion

namespace TMS.Common.Network
{
	public interface INetworkDispatcherConfiguration : IBaseConfiguration
	{
		NetworkRequestConfigurationData RequestsConfig { get; set; }
	}
}