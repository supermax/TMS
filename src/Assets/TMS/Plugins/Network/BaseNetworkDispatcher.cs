using TMS.Common.Logging;
using TMS.Common.Extensions;
using TMS.Common.Messaging;
using TMS.Common.Modularity;

namespace TMS.Common.Network
{
	// TODO add efficient log handling method
	public abstract class BaseNetworkDispatcher : INetworkDispatcher, IMessengerConsumer
	{
		public virtual void Subscribe()
		{
			Messenger.Default.Subscribe<BaseServiceRequestPayload>(OnServiceRequest);
		}

		public abstract INetworkDispatcherConfiguration Configuration { get; }

		public abstract IServiceFactory ServiceRequestTaskFactory { get; }

		public virtual void Publish(BaseServiceRequestPayload payload)
		{
			Messenger.Default.Publish(payload);
		}

		public virtual IServiceRequestTask Process(BaseServiceRequestPayload payload)
		{
			if (!payload.IsCustomService && !Configuration.RequestsConfig.Services.ContainsKey(payload.ServiceName))
			{
				var error = string.Format("Cannot find service with name \"{0}\"", payload.ServiceName);
				// TODO write to logger
#if UNITY3D || UNITY_3D
				Loggers.Default.NetworkLogger.Write(LogSourceType.Error, string.Format("ERROR: {0}->Process({1})", GetType(), error));
				
#else
				System.Diagnostics.Debug.WriteLine("ERROR: {0}->Process({1})", GetType(), error); // TODO
#endif
				OnServiceRequestError(payload, error);
				return null;
			}

			if (payload.IsCustomService)
			{
				if (payload.CustomServiceUrl.IsNullOrEmpty())
				{
					var error = string.Format("Custom service URL is NULL\\Empty \"{0}\"", 
												payload.ServiceName.IsNullOrEmpty() ? payload.GetType().Name : payload.ServiceName);
					// TODO write to logger
#if UNITY3D || UNITY_3D
					Loggers.Default.NetworkLogger.Write(LogSourceType.Error, string.Format("ERROR: {0}->Process({1})", GetType(), error));

#else
					System.Diagnostics.Debug.WriteLine("ERROR: {0}->Process({1})", GetType(), error); // TODO
#endif
					OnServiceRequestError(payload, error);
					return null;
				}

				payload.Config = new NetworkRequestConfiguration
								{
									RequestMaxRetriesNum = Configuration.RequestsConfig.Default.RequestMaxRetriesNum,
									RequestTimeout = Configuration.RequestsConfig.Default.RequestTimeout,
									ServiceUrl = payload.CustomServiceUrl
								};
				if (payload.ServiceName.IsNullOrEmpty())
				{
					payload.ServiceName = payload.CustomServiceUrl;
				}
			}
			else
			{
				payload.Config = Configuration.RequestsConfig.Services[payload.ServiceName];
			}
			var task = ServiceRequestTaskFactory.CreateTask(payload);

			task.Start();
			return task;
		}

		protected virtual void OnServiceRequestError(BaseServiceRequestPayload requestPayload, string error)
		{
			var responsePayload = IocManager.Default.Resolve<IServiceFactory>()
				.CreateResponsePayload(requestPayload, string.Empty, ServiceResponseState.AbortedByClient, error);
			if (responsePayload == null)
			{
#if UNITY3D || UNITY_3D
				Loggers.Default.NetworkLogger.Write(LogSourceType.Error, 
					string.Format("ERROR: {0}->OnServiceRequestError(Cannot create response payload for service '{1}')", 
										GetType(), requestPayload.ServiceName));
#else
				System.Diagnostics.Debug.WriteLine("ERROR: {0}->OnServiceRequestError(Cannot create response payload for service '{1}')",
										GetType(), requestPayload.ServiceName); // TODO
#endif
				return;
			}

			if (requestPayload.ResponseCallbackId.IsNullOrEmpty())
			{
				Messenger.Default.Publish(responsePayload);
				return;
			}

			Messenger.Default.PublishAndUnsubscribe(responsePayload, requestPayload.ResponseCallbackId);
		}

		protected virtual void OnServiceRequest(BaseServiceRequestPayload payload)
		{
			if (payload.Abort || payload.AbortAll) return;
			Process(payload);
		}
	}
}