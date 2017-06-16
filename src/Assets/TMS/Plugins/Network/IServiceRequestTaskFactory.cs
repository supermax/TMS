#if UNITY3D || UNITY_3D
using UnityEngine;
#endif

namespace TMS.Common.Network
{
	public interface IServiceFactory
	{
		IServiceRequestTask CreateTask(BaseServiceRequestPayload payload);

		TP CreateRequestPayload<TP, TD>(string serviceName, TD data)
			where TP : BaseServiceRequestPayload<TD>;

		TP CreateRequestPayload<TP>(string serviceName, object data = null)
			where TP : BaseServiceRequestPayload;

		TP CreateResponsePayload<TP, TD>(BaseServiceRequestPayload parent, string respStr, ServiceResponseState state,
			string error = null)
			where TP : BaseServiceResponsePayload<TD>;

		BaseServiceResponsePayload CreateResponsePayload(BaseServiceRequestPayload parent, string respStr,
			ServiceResponseState state, string error = null);

#if UNITY3D || UNITY_3D
		BaseServiceResponsePayload CreateResponsePayload(BaseServiceRequestPayload parent, Texture2D image,
			ServiceResponseState state, string error = null);

		TP CreateResponsePayload<TP, Texture2D>(BaseServiceRequestPayload parent, Texture2D image, ServiceResponseState state,
			string error = null)
			where TP : BaseServiceResponsePayload<Texture2D>;
#endif
	}
}