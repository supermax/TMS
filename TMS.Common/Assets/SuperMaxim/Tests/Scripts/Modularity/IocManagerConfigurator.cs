using TMS.Common.Modularity;
using TMS.Common.Modularity.Ioc;
using UnityEngine;

namespace Inception.Core.Assets.Plugins.Internal.Core.Plugins.External.TMS.Tests.Scripts.Modularity
{
	public class IocManagerConfigurator : MonoBehaviour
	{
		void Awake()
		{
			IocManager.Default.Configure(GetType());
		}
	}
}