#region

using TMS.Common.Config;
using TMS.Common.Network.Request.Api;

#endregion

namespace TMS.Common.Network.Config.Api
{
	public interface INetworkConfiguration : IBaseConfiguration
	{
		NetworkRequestConfigurationData RequestsConfig { get; }		
	}
}