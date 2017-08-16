#region

using TMS.Common.Core;

#endregion

namespace TMS.Common.Network
{
	internal class ServiceRequestTaskDispatcher : MonoBehaviorBaseSingleton<ServiceRequestTaskDispatcher>
	{
		protected override void OnDestroy()
		{
			StopAllCoroutines();
			base.OnDestroy();
		}
	}
}