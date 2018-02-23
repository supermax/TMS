using TMS.Common.Modularity;
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