#region

using TMS.Common.Messaging;
using TMS.Common.Modularity;

#endregion

namespace TMS.Common.Network
{
	[MessengerConsumer(typeof (INetworkDispatcher), true, InstantiateOnRegistration = true)]
	public class NetworkDispatcher : BaseNetworkDispatcher
	{
		#region Overrides of BaseNetworkDispatcher

		public override INetworkDispatcherConfiguration Configuration
		{
			get
			{
				var config =  IocManager.Default.Resolve<INetworkDispatcherConfiguration>();
				return config;
			}
		}

		public override IServiceFactory ServiceRequestTaskFactory
		{
			get
			{
				var factory = IocManager.Default.Resolve<IServiceFactory>();
				return factory;
			}
		}

		#endregion
	}
}