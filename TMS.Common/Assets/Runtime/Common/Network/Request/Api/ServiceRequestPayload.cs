#region Usings

using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Network.Request.Api
{
	public class ServiceRequestPayload<T> : BaseServiceRequestPayload, IServiceRequestPayload<T>
	{
		public ServiceRequestPayload(string serviceName, T data)
			: base(serviceName)
		{
			RequestData = data;
		}

		[JsonDataMember("data")] // TODO rename to "Response"
		public virtual T RequestData { get; set; }
	}
}