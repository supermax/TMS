using UnityEngine;

namespace TMS.Common.Modularity
{
	public class IocManagerConfigurator : MonoBehaviour
	{
		void Awake()
		{
			IocManager.Default.Configure(GetType());
		}
	}
}