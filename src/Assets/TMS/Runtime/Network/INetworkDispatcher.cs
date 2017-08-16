namespace TMS.Common.Network
{
	public interface INetworkDispatcher
	{
		INetworkDispatcherConfiguration Configuration { get; }
		IServiceFactory ServiceRequestTaskFactory { get; }
		void Publish(BaseServiceRequestPayload payload);
		IServiceRequestTask Process(BaseServiceRequestPayload payload);
	}
}