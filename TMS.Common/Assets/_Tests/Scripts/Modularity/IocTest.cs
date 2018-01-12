using System;
using UnityEngine;

namespace TMS.Common.Modularity
{
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
}