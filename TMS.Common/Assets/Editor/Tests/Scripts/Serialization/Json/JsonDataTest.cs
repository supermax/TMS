using System.Collections;
using TMS.Common.Serialization.Json;
using System.IO;
using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using TMS.Common.Extensions;
using TMS.Common.Serialization.Json.Api;
using TMS.Common.Tests.Serialization.Json.TestClasses;
using TMS.Common.Tests.Serialization.Json.TestResources;

namespace TMS.Common.Tests.Serialization.Json
{
	[TestFixture]
	public class JsonDataTest
	{
		#region Fields
		#endregion

		#region Constructors
		#endregion

		#region Tests

		[Test]
		public void JsonData_State_Int()
		{
			const int value = 273;
			var jsonData = new JsonData(value);
			Assert.IsTrue(jsonData.IsInt);
		}

		[Test]
		public void JsonData_Convertion_Explicit_Int()
		{
			const int value = 273;
			var jsonData = new JsonData(value);
			Assert.AreEqual((int)jsonData, value);
		}

		[Test]
		public void JsonData_State_Double()
		{
			const double value = 34.38;
			var jsonData = new JsonData(value);
			Assert.IsTrue(jsonData.IsDouble);
		}

		[Test]
		public void JsonData_Convertion_Explicit_Double()
		{
			const double value = 34.38;
			var jsonData = new JsonData(value);
			Assert.AreEqual((double)jsonData, value);
		}

		[Test]
		public void JsonData_State_Boolean()
		{
			const bool value = true;
			var jsonData = new JsonData(value);
			Assert.IsTrue(jsonData.IsBoolean);
		}

		[Test]
		public void JsonData_Convertion_Explicit_Boolean()
		{
			const bool value = true;
			var jsonData = new JsonData(value);
			Assert.AreEqual((bool)jsonData, value);
		}

		[Test]
		public void JsonData_State_String()
		{
			const string value = StringResources.TestString;
			var jsonData = new JsonData(value);
			Assert.IsTrue(jsonData.IsString);
		}

		[Test]
		public void JsonData_Convertion_Explicit_String()
		{
			const string value = StringResources.TestString;
			var jsonData = new JsonData(value);
			Assert.AreEqual((string)jsonData, value);
		}

		[Test]
		public void JsonData_State_Long()
		{
			const long value = 43324L;
			var jsonData = new JsonData(value);
			Assert.IsTrue(jsonData.IsLong);
		}

		[Test]
		public void JsonData_Convertion_Explicit_Long()
		{
			const long value = 43324L;
			var jsonData = new JsonData(value);
			Assert.AreEqual((long)jsonData, value);
		}

		[Test]
		public void JsonData_Convertation_ToString()
		{
			var jsonData = JsonMapper.Default.ToObject(StringResources.PersonArray);
			var s = jsonData.ToJson();
			Assert.IsFalse(string.IsNullOrEmpty(s));
		}

		public TestContext TestContext { get; set; }

		[Test]
		public void ReadEmpireJson()
		{
			var str = File.ReadAllText(@"E:\ws\dev\unity\infr\TMS\TMS\TMS.Common.Test\Serialization\Json\TestResources\_FullEmpireTest.json");

			var time = DateTime.Now;

			var res = JsonMapper.Default.ToObject(str);

			TestContext.WriteLine("Parsing time: " + (DateTime.Now - time).TotalMilliseconds + " ms");

			Assert.IsNotNull(res);
		}

		[Test]
		public void ReadEmpireJsonAsClass()
		{
			var str = File.ReadAllText(@"E:\ws\dev\unity\infr\TMS\TMS\TMS.Common.Test\Serialization\Json\TestResources\_FullEmpireTest.json");

			var time = DateTime.Now;

			var empire = JsonMapper.Default.ToObject<Empire>(str);

			TestContext.WriteLine("Parsing time: " + (DateTime.Now - time).TotalMilliseconds + " ms");

			Assert.IsNotNull(empire);
		}

		[Test]
		public void ReadEmpireJsonAsDic()
		{
			var str = File.ReadAllText(@"E:\ws\dev\unity\infr\TMS\TMS\TMS.Common.Test\Serialization\Json\TestResources\_FullEmpireTest.json");

			var time = DateTime.Now;

			var empire = JsonMapper.Default.ToObject(str);

			var hero = JsonMapper.Default.ToObject<Hero>(empire["hero"]);
			var city = JsonMapper.Default.ToObject<City>(empire["city"]);

			if (empire["city"][0].IsInt)
			{
				var id = (int) empire["city"][0];
				id = empire["city"][0].Cast<int>().FirstOrDefault();
			}

			str = empire.ToJson();
			
			TestContext.WriteLine("Parsing time: " + (DateTime.Now - time).TotalMilliseconds + " ms");

			Assert.IsNotNull(empire);
		}

		/// <summary>
		/// Writes the empire json as class.
		/// </summary>
		[Test]
		public void WriteEmpireJsonAsClass()
		{
			Empire empire = null, empire1, empire2;

			var str = File.ReadAllText(@"E:\ws\dev\unity\infr\TMS\TMS\TMS.Common.Test\Serialization\Json\TestResources\_FullEmpireTest.json");

			if (!str.IsNullOrEmpty())
			{
				empire = JsonMapper.Default.ToObject<Empire>(str);
			}

			var time = DateTime.Now;

			if(time < DateTime.Now) return;
			

			TestContext.WriteLine("Parsing time: " + (DateTime.Now - time).TotalMilliseconds + " ms");

			str = JsonMapper.Default.ToJson(empire);

			var ary = new JsonData();
			for (int i = 0; i < 20; i++)
			{
				ary.Add(i);
			}

			var data = new JsonData
			{
				{"a", new JsonData
							{
								{"b",  1},
								{"c", "str"}
							}},
				{"ary", ary }
			};
			data.ToJson();

			Assert.IsNotNull(empire);
		}

		[Test]
		public void ReadConfigFileJson()
		{
			var jsonStr =
				File.ReadAllText(
					@"Z:\SuperMax\Git\swar\skywarsarchonrises\unity\infr\TMS\TMS\TMS.Common.Test\Serialization\Json\TestResources\_Config.json");
			var jsonData = JsonMapper.Default.ToObject(jsonStr);
			Assert.IsNotNull(jsonData);
		}

		[Test]
		public void ReadNetworkConfigFileJson()
		{
			var jsonStr =
				File.ReadAllText(
					@"Z:\SuperMax\Git\swar\skywarsarchonrises\unity\infr\TMS\TMS\TMS.Common.Test\Serialization\Json\TestResources\_NetworkConfig.json");
			var jsonData = JsonMapper.Default.ToObject(jsonStr);
			Assert.IsNotNull(jsonData);

			var config = JsonMapper.Default.ToObject<NetworkConfig>(jsonStr);
			Assert.IsNotNull(config);
		}

		#endregion
	}
}
