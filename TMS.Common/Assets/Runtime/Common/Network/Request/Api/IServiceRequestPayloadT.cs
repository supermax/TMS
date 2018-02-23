namespace TMS.Common.Network.Request.Api
{
	public interface IServiceRequestPayload<T> : IServiceRequestPayload
	{
		T RequestData { get; set; }
	}
}