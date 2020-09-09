namespace TMS.Common.Network.Response.Api
{
	public interface IServiceResponsePayload<T> : IServiceResponsePayload
	{
		T ResponseData { get; set; }
	}
}