using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootsrapper : MonoBehaviour {

	// Use this for initialization
	void Start () {
		TMS.Common.Modularity.IocManager.Default.Configure(GetType());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public IMessagePrinter Printer
	{
		get
		{
			var printer = TMS.Common.Modularity.IocManager.Default.Resolve<IMessagePrinter>();
			return printer;
		}
	}
}
