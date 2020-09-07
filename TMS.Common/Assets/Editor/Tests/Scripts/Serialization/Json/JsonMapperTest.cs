#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;
using TMS.Common.Tests.Serialization.Json.TestClasses;
using TMS.Common.Tests.Serialization.Json.TestResources;

#endregion

namespace TMS.Common.Test.Serialization.Json
{
	[TestFixture]
	public sealed class JsonMapperTest
	{
		#region Fields

		private string _jsonTest;

		#endregion

		#region Methods

		[SetUp]
		public void Startup()
		{
			//_jsonTest = File.ReadAllText(FileResources.JsonTestFilePath);

			_jsonTest = FileResources.GetJsonTextAsset().text;
		}

		#endregion

		#region Tests

		[Test]
		public void JsonMapper_State_SingletonProperty_NotNull()
		{
			var mapper = JsonMapper.Default;
			Assert.IsNotNull(mapper);
		}

		[Test]
		public void JsonMapper_Convertation_ClassProperties_Setting()
		{
			const string name = "person_name";
			const string surname = "person_surname";
			const int age = 27;

			var personJsonString = StringResources.GetPerametrizedPerson(name, surname, age);
			var jsonDataPerson = JsonMapper.Default.ToObject(personJsonString);

			var person = JsonMapper.Default.ToObject<Person>(jsonDataPerson);

			jsonDataPerson = JsonMapper.Default.ToJson(person);

			var jd = new JsonData
			{
				{"name", "Maxim"},
				{"ver", 1.0f},
				{"person", jsonDataPerson},
				{"arr", new JsonData {new int[] {1, 2, 3}}}
			};
			
			// TODO
			var str = jd.ToJson();
			Assert.NotNull(str);

			Assert.AreEqual(name, person.Name);
			Assert.AreEqual(surname, person.Surname);
			Assert.AreEqual(age, person.Age);
		}

		[Test]
		public void JsonMapper_Conversion_GetJsonDataObjectFromString_NotNull()
		{
			var person = JsonMapper.Default.ToObject(StringResources.Person);
			Assert.IsNotNull(person);
		}

		[Test]
		public void JsonMapper_Conversion_JsonDataObjectToSpecifiedType_NotNull()
		{
			var jsonDataPerson = JsonMapper.Default.ToObject(StringResources.Person);
			var person = JsonMapper.Default.ToObject<Person>(jsonDataPerson);
		}

		[Test]
		public void JsonMapper_Conversion_GetArrayOfJsonData_WithRightCount()
		{
			var persons = JsonMapper.Default.ToObject(StringResources.PersonArray);
			Assert.IsNotNull(persons);
			Assert.AreEqual(persons.Count, StringResources.PersonsArrayCount);
		}

		[Test]
		public void JsonMapper_Conversion_CountOfObjectsProperties_EqualsToTypesPropertiesCount()
		{
			var jsonDataPerson = JsonMapper.Default.ToObject(StringResources.Person);
			Assert.AreEqual(jsonDataPerson.Count, StringResources.PersonPropertiesCount);
		}

		[Test]
		public void JsonMapper_Conversion_ArrayOfJsonData_ToSpecifiedType_RightElementCount()
		{
			var jsonDataPersons = JsonMapper.Default.ToObject(StringResources.PersonArray);
			var persons = JsonMapper.Default.ToObject<Person[]>(jsonDataPersons);
			Assert.AreEqual(jsonDataPersons.Count, persons.Length);
		}

		[Test]
		public void JsonMapper_Conversion_LoginDataFormatToJsonDataObject()
		{
			var loginJsonData = JsonMapper.Default.ToObject(_jsonTest);
			Assert.IsNotNull(loginJsonData);
		}

		[Test]
		public void JsonMapper_Conversion_LoginDataFormatToClass()
		{
			//var loginJsonData = JsonMapper.Default.ToObject(_jsonTest);
			//Assert.IsNotNull(loginJsonData);

			//var lvl = (int) loginJsonData["Empire"]["2010"][0]["lvl"];
			//var obj = JsonMapper.Default.ToObject<MeRequestData>(loginJsonData);

			JsonMapper.Default.RegisterImporter<IList<MeRequestData>,List<MeRequestData>>(lst => new List<MeRequestData>(lst));
			
			var obj = JsonMapper.Default.ToObject<MeRequestData>(_jsonTest);

			Assert.IsNotNull(obj);
		}

		[Test]
		public void JsonMapper_Conversion_Subclasses_Initialized()
		{
			//var jsonDataJsonClass = JsonMapper.Default.ToObject(StringResources.JsonClass);
			var jsonClass = JsonMapper.Default.ToObject<JsonClass>(StringResources.JsonClass);

			Assert.IsNotNull(jsonClass.Child);
			Assert.IsNotNull(jsonClass.SubChild);
			Assert.AreEqual(5, jsonClass.SomeAnotherInt); // = 5 bcs failed to cast from dbl to int
		}

		[Test]
		public void JsonMapper_Conversion_LoginDataFormat()
		{
			var loginJsonData = JsonMapper.Default.ToObject(_jsonTest);
			Assert.AreEqual(loginJsonData.Count, 12);
			var o1 = loginJsonData[0];
			var o2 = loginJsonData[1];

			var spinResult = JsonMapper.Default.ToObject<SpinResultObj>(o1);
			var currency = JsonMapper.Default.ToObject<UserCurrencyObj>(o2);

			Assert.IsNotNull(spinResult);
			Assert.IsNotNull(currency);
		}

		internal class SomeClass
		{
			public DateTime date { get; set; }

			[JsonDataMember("processTime")]
			public long MyProcessTime { get; set; }

			public bool sucess { get; set; }

			public SomeErrorClass error { get; set; }

			public int serverMessage { get; set; }
		}

		internal class SomeErrorClass
		{
			public string error { get; set; }
		}

		[Test]
		public void JsonMapper_Deserialize_NullObject()
		{
			const string jsonStr = "{\"date\":\"2015-03-20 03:45:45\", \"processTime\":7, \"success\":true, \"error\":null, \"serverMessage\":0}";
			var obj = JsonMapper.Default.ToObject<SomeClass>(jsonStr);
			Assert.IsNotNull(obj);
			
			var jsonObj = JsonMapper.Default.ToObject(jsonStr);
			Assert.IsNotNull(jsonObj);
		}

		[Test]
		public void JsonMapper_Deserialize_Convert()
		{
			//JsonMapper.Default.RegisterImporter();
		}

//		public void TestDeserialize()
//		{
//			//const string jstr = "{\"name\": \"maxim\", \"id\": 1, \"num\": 0.123, \"another\" : 234, \"subsub\" : {\"num1\":10, \"str1\" : \"hello\"}, \"sub\": { \"num\": 0.00054, \"parent\": 2 }, \"lastint\": 11}";

//			var jObj = JsonMapper.Default.ToObject(StringResources.JsonClass);
//			Assert.IsNotNull(jObj);

//			//var r1 = JsonMapper.Default.ReadValue(typeof(JsonClass), jObj);
//			var r1 = JsonMapper.Default.ToObject<JsonClass>(jObj);
//			Assert.IsNotNull(r1);

//			//const string jclassarray = "[{\"name\":\"pavel\", \"sorname\":\"rumyancev\", \"age\" : 27, \"numbers\" : [1,2,3,4,5,4,3,21]}, {\"name\":\"marina\", \"sorname\":\"rumyancev\", \"age\" : 25, \"numbers\" : [9,8,7,6,5,4,3,2,1]}]";
//			var jClassArrayObject = JsonMapper.Default.ToObject(StringResources.PersonArray);
//			var r2 = JsonMapper.Default.ToObject<Person[]>(jClassArrayObject);
//			Assert.IsNotNull(r2);

//			var obj = JsonMapper.Default.ToObject<JsonClass>(StringResources.JsonClass);
//			Assert.IsNotNull(obj);

//			var obj1 = JsonMapper.Default.ToObject<JsonClass>(jObj);
//			Assert.IsNotNull(obj1);

			
//			//const string jperson = "{\"name\":\"pavel\", \"sorname\":\"rumyancev\", \"age\" : 27}";
//			var jObj2 = JsonMapper.Default.ToObject(StringResources.Person);
//			var person = JsonMapper.Default.ToObject<Person>(jObj2);

//			var j = JsonMapper.Default.ToJson(person);
//			var j2 = JsonMapper.Default.ToJson(r2);

//			const string jarray = "[\"one\",\"two\",\"three\",\"four\"]";
//			var jObj3 = JsonMapper.Default.ToObject(jarray);
//			var jObj4 = JsonMapper.Default.ToObject<string[]>(jObj3);

//			var res = JsonMapper.Default.ToObject<MachinesObj>(StringResources.JsonClass);
//			Assert.IsNotNull(res);

//			var res1 = JsonMapper.Default.ToObject(StringResources.JsonClass);
//			var jObj5 = JsonMapper.Default.ToObject<MachinesObj>(res1);


//			var aryJstr = File.ReadAllText(@"r:\winity\Infr\diwip.Infr\TMS.Common.Test\Serialization\Json\TestClasses\SlotsClubLogin.txt");
//			var jObj1 = JsonMapper.Default.ToObject(aryJstr);
//			var o1 = jObj1[0];
//			var o2 = jObj1[1];

//			var resultO1 = JsonMapper.Default.ToObject<SpinResultObj>(o1);
//			var resultO2 = JsonMapper.Default.ToObject<UserCurrencyObj>(o2);
////--

//			//var a1 = JsonMapper.ToObject<SpinResultObj>(o1);

//			//var emptyAry = JsonMapper.ToObject<SpinResultObj>(aryJstr);
//			//Assert.IsNotNull(emptyAry);

//			////var types = new Dictionary<string, Type>
//			////{
//			////	{"spin", typeof (SpinResultObj)},
//			////	{"GetUserCurrency", typeof (UserCurrencyObj)}
//			////};			

//			////var wrapper = JsonMapper.ToObject(aryJstr);
//			////foreach (IJsonWrapper item in wrapper)
//			////{
//			////	if (!((IDictionary)item).Contains("cmd")) continue;

//			////	var cmd = ((IJsonWrapper)item["cmd"]).GetString();
//			////	var objType = types[cmd];

//			////	var json = wrapper.ToJson();

//			////	var reader = new JsonReader(json);
//			////	var res = JsonMapper.ReadValue(objType, reader);
//			////}

//			//var reader = new JsonReader(aryJstr);
//			//while (reader.Read())
//			//{
//			//	switch (reader.Token)
//			//	{
//			//		case JsonToken.ObjectEnd:
//			//			break;
//			//	}
//			//}
//		}

		#endregion
	}
}