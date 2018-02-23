using System.Collections.Generic;
using NUnit.Framework;
using TMS.Common.Modularity;
using UnityEngine;

namespace TMS.Common.Test.Modularity
{
	[TestFixture]
	public class IocManagerTest
	{
		[OneTimeSetUp]
		public void Setup()
		{
			IocManager.Default.Configure(GetType());
		}

		[Test]
		public void TestAssemblyConfig()
		{
			var mySql = new MySQLite();
			IocManager.Default.Register<ILocalStorage>(mySql);

			IocManager.Default.Register<ILocalStorage>(typeof(MySQLite), true);


			var test = IocManager.Default.Resolve<ITestIoc>();
			Assert.NotNull(test);

		}

		[Test]
		public void TestResolve()
		{
			var i1 = IocManager.Default.Resolve<ITestIoc>(typeof(TestIoc1));
			Assert.IsNotNull(i1);
			Assert.AreEqual(i1.GetType(), typeof (TestIoc1));

			var i2 = IocManager.Default.Resolve<ITestIoc>(typeof(TestIoc2));
			Assert.IsNotNull(i2);
			Assert.AreEqual(i2.GetType(), typeof(TestIoc2));
		}

		[Test]
		public void TestSQLBridge()
		{
			var storageBridge = IocManager.Default.Resolve<ILocalStorage>();
			Assert.NotNull(storageBridge);

			//var result = storageBridge.Select<string>("SELECT * FROM USERS");
		}
	}
	
	public interface ITestIoc
	{

	}

	[IocTypeMap(true, true)]
	public class TestIoc2 : ITestIoc
	{
		public TestIoc2()
		{
			Debug.LogFormat("{0}->CTOR", typeof(TestIoc2));
		}
	}

	[IocTypeMap(true, true)]
	public class TestIoc1 : ITestIoc
	{
		public TestIoc1()
		{
			Debug.LogFormat("{0}->CTOR", typeof(TestIoc1));
		}
	}

	

	#if UNITY_WEB
	[IocTypeMap(typeof(ILocalStorage), true, InstantiateOnRegistration = true)]
	public class MyCanvasLocalStorage : ILocalStorage
	{
		#region Implementation of ILocalStorage

		public IEnumerable<T> Select<T>(string query)
		{
			yield break;
		}

		#endregion
	}
	#else
	[IocTypeMap(typeof(ILocalStorage), true, InstantiateOnRegistration = true)]
	public class MySQLite : ILocalStorage
	{
		#region Implementation of ILocalStorage

		public IEnumerable<T> Select<T>(string query)
		{
			yield break;
		}

		#endregion

		//public MySQLite(ITestIoc testInstance, IFacebookManager fb, StringBuilder str)
		//{
		//	testInstance.ToString(); //IocManager.Default.Resolve<ITestIoc>().ToString();
		//}
	}
	#endif

	public interface ILocalStorage
	{
		IEnumerable<T> Select<T>(string query);
	}
}