#region

using System;
using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
	public class Person
	{
		[JsonDataMember("name")] public string Name { get; set; }

		[JsonDataMember("surname")] public string Surname { get; set; }

		[JsonDataMember("age")] public int Age { get; set; }

		[JsonDataMember("numbers")] public int[] Numbers { get; set; }
	}

	public class JsonClass
	{
		[JsonDataMemberIgnore] public string name { get; set; }

		public int id { get; set; }

		public DateTime time { get; set; }

		[JsonDataMember("num")] public double SomeNumber { get; set; }

		[JsonDataMember("another", FallbackValue = 5)]
		public int SomeAnotherInt { get; set; }

		[JsonDataMember("subsub")] public JsonSubSubClass SubChild { get; set; }

		[JsonDataMember("sub")] public JsonSubClass Child { get; set; }

		[JsonDataMember("lastint")] public int LastInt { get; set; }
	}

	public class JsonSubClass
	{
		public double num { get; set; }

		public int parent { get; set; }
	}

	public class JsonSubSubClass
	{
		public int num1 { get; set; }

		public string str1 { get; set; }
	}
}