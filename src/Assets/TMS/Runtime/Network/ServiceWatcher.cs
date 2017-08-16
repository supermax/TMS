using System;
using TMS.Common.Core;
using TMS.Common.Helpers;
using TMS.Common.Messaging;
using TMS.Common.Modularity;

namespace TMS.Common.Network
{
	public interface IServiceWatcher
	{
		ServiceRequestPingPayload CheckServiceStateAsync(NetworkRequestConfiguration svcConfig, ServiceRequestPingType pingType, Action<ServiceStateCheckResultPayload> callback);
	}

	[MessengerConsumer(typeof(IServiceWatcher), true, InstantiateOnRegistration = true, AutoSubscribe = true)]
	public class ServiceWatcher : Singleton<IServiceWatcher, ServiceWatcher>, IServiceWatcher, IMessengerConsumer
	{
		public virtual ServiceRequestPingPayload CheckServiceStateAsync(NetworkRequestConfiguration svcConfig, 
			ServiceRequestPingType pingType, Action<ServiceStateCheckResultPayload> callback)
		{
			// TODO
			var configClone = DataMapper.Default.Clone(svcConfig);
			switch (pingType)
			{
				case ServiceRequestPingType.Once:
					configClone.RequestMaxRetriesNum = 1;
					break;
			}
			var payload = IocManager.Default.Resolve<IServiceFactory>().CreateRequestPayload<ServiceRequestPingPayload>(configClone.ServiceName);
			payload.RequestPingType = pingType;
			payload.Config = configClone;
			Messenger.Default.Publish<BaseServiceRequestPayload>(payload);
			return payload;
		}

		public void Subscribe()
		{
			Messenger.Default.Subscribe<ServiceStateCheckResultPayload>(OnCheckServiceState);
		}

		private void OnCheckServiceState(ServiceStateCheckResultPayload payload)
		{
			switch (payload.ParentRequestPayload.RequestPingType)
			{
				case ServiceRequestPingType.Continuously:
					payload.ParentRequestPayload.Abort = false;
					payload.ParentRequestPayload.AbortAll = false;
					Messenger.Default.Publish<BaseServiceRequestPayload>(payload.ParentRequestPayload);
					break;
			}
		}
	}

	[ServicePayloadMapping(typeof(ServiceStateCheckResultPayload))]
	public class ServiceRequestPingPayload : BaseServiceRequestPayload
	{
		public virtual ServiceRequestPingType RequestPingType { get; protected internal set; }

		public ServiceRequestPingPayload(string serviceName) : base(serviceName)
		{
			
		}

		public ServiceRequestPingPayload(NetworkRequestConfiguration svcConfig)
			: this(svcConfig.ServiceName)
		{
			Config = svcConfig;
			IsCustomService = true;
			CustomServiceUrl = Config.ServiceUrl;
		}
	}

	public enum ServiceRequestPingType
	{
		Once = 0,
		ConfigBased,
		Continuously
	}

	public class ServiceStateCheckResultPayload : BaseServiceResponsePayload
	{
		public new ServiceRequestPingPayload ParentRequestPayload
		{
			get
			{
				return base.ParentRequestPayload as ServiceRequestPingPayload;
			}
			set
			{
				base.ParentRequestPayload = value;
			}
		}

		public ServiceStateCheckResultPayload(BaseServiceRequestPayload parentRequestPayload, ServiceResponseState state, string error = null) 
			: base(parentRequestPayload, state, error)
		{

		}
	}
}