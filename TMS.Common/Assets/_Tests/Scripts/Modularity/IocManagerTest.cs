using TMS.Common.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TMS.Common.Modularity
{
	public class IocManagerTest : ViewModel
	{
		[SerializeField] private IocTest _instance;

		[SerializeField] private Text _buttonText;	

		public void Configure()
		{
			var type = GetType();

			Debug.Log("*** Configure Assembly START >> " + type);

			IocManager.Configure(type);

			_buttonText.text = "DONE";

			Debug.Log("*** Configure Assembly END");
		}

		public void Resolve()
		{
			Debug.Log("*** Resolve START");

			_instance = (IocTest)Resolve<IIocTest>();
			var instanceName = string.Format("{0} ({1})", _instance.Name, _instance.GetHashCode());
			_buttonText.text = instanceName;

			Debug.Log("*** Resolve END >> " + instanceName);
		}
	
	}
}