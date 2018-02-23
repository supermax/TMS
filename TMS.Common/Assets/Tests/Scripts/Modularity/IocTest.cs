using System;
using TMS.Common.Modularity;
using UnityEngine;

[Serializable]
[IocTypeMap(typeof(IIocTest), true)]
public class IocTest : IIocTest
{
	[SerializeField]
	private string _name;

	public string Name
	{
		get { return _name; }
		set { _name = value; }
	}

	public IocTest()
	{
		Name = typeof(IocTest).FullName;
	}
}