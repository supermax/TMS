using TMS.Common.Modularity;
using UnityEngine;

public class Bootsrapper : MonoBehaviour
{

	public IAppContext AppCtxt
	{
		get
		{
			var a = IocManager.Default.Resolve<IAppContext>();
			return a;
		}
	}

	public IMessagePrinter Printer
	{
		get
		{
			var printer = IocManager.Default.Resolve<IMessagePrinter>();
			return printer;
		}
	}
	
	void Start ()
	{
		IocManager.Default.Configure(GetType());
		
		IocManager.Default.Register<IAppContext>(typeof(AppContext), true);
	}
}
