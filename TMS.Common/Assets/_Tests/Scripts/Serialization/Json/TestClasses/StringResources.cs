namespace TMS.Common.Tests.Serialization.Json.TestResources
{
	internal sealed class StringResources
	{
		#region Properties

		#region Mechines[]

		public const string MachinesArray
			= @"{
			""machines"": [{
				""id"": 1,
				""o"": 1,
				""n"": ""Arabian Nights"",
				""v"": 4,
				""unlockedLvl"": 1,
				""mt"": ""arabian"",
				""br"": 1,
				""rank"": 4.440299987792969,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 2,
				""o"": 4,
				""n"": ""Wild West"",
				""v"": 4,
				""unlockedLvl"": 10,
				""mt"": ""wildwest"",
				""br"": 1,
				""rank"": 4.405600070953369,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 3,
				""o"": 9,
				""n"": ""Jungle Fever"",
				""v"": 4,
				""unlockedLvl"": 27,
				""mt"": ""amazon"",
				""br"": 1,
				""rank"": 4.446599960327148,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 4,
				""o"": 10,
				""n"": ""Medieval Crown"",
				""v"": 4,
				""unlockedLvl"": 30,
				""mt"": ""medieval"",
				""br"": 1,
				""rank"": 4.403600215911865,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 5,
				""o"": 3,
				""n"": ""Atlantis"",
				""v"": 4,
				""unlockedLvl"": 7,
				""mt"": ""atlantis"",
				""br"": 1,
				""rank"": 4.343200206756592,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 6,
				""o"": 12,
				""n"": ""China"",
				""v"": 4,
				""unlockedLvl"": 38,
				""mt"": ""china"",
				""br"": 1,
				""rank"": 4.469099998474121,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 7,
				""o"": 5,
				""n"": ""Disco Madness"",
				""v"": 4,
				""unlockedLvl"": 13,
				""mt"": ""disco"",
				""br"": 1,
				""rank"": 4.385000228881836,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 8,
				""o"": 8,
				""n"": ""Grimm's Forest"",
				""v"": 4,
				""unlockedLvl"": 24,
				""mt"": ""grimm"",
				""br"": 1,
				""rank"": 4.5030999183654785,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 9,
				""o"": 11,
				""n"": ""Vikings"",
				""v"": 4,
				""unlockedLvl"": 33,
				""mt"": ""vikings"",
				""br"": 1,
				""rank"": 4.392499923706055,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 10,
				""o"": 7,
				""n"": ""Fifties"",
				""v"": 4,
				""unlockedLvl"": 21,
				""mt"": ""fifties"",
				""br"": 1,
				""rank"": 4.455599784851074,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 11,
				""o"": 2,
				""n"": ""Farm"",
				""v"": 4,
				""unlockedLvl"": 4,
				""mt"": ""farm"",
				""br"": 1,
				""rank"": 4.299300193786621,
				""userRank"": -1,
				""isActive"": true
			},
			{
				""id"": 12,
				""o"": 6,
				""n"": ""Pirates"",
				""v"": 4,
				""unlockedLvl"": 18,
				""mt"": ""pirates"",
				""br"": 1,
				""rank"": 4.442599773406982,
				""userRank"": -1,
				""jp"": 292973,
				""isActive"": true
			},
			{
				""id"": 13,
				""o"": 13,
				""n"": ""Ice"",
				""v"": 4,
				""unlockedLvl"": 99,
				""mt"": ""ice"",
				""br"": 1,
				""rank"": -1,
				""userRank"": -1,
				""isActive"": false
			},
			{
				""id"": 14,
				""o"": 14,
				""n"": ""Mafia"",
				""v"": 4,
				""unlockedLvl"": 99,
				""mt"": ""mafia"",
				""br"": 0,
				""rank"": -1,
				""userRank"": -1,
				""isActive"": false
			}],
			""page"": 1,
			""cmd"": ""GetMachines""
		}";

		#endregion

		#region JsonClass

		public const string JsonClass =
			"{\"name\": \"maxim\", \"id\": 1, \"num\": 0.123, \"another\" : 2.34, \"subsub\" : {\"num1\":10, \"str1\" : \"hello\"}, \"sub\": { \"num\": 0.00054, \"parent\": 2 }, \"lastint\": 11}";

		#endregion

		#region Person[]

		public const string PersonArray =
			"[{\"name\":\"pavel\", \"sorname\":\"rumyancev\", \"age\" : 27, \"numbers\" : [1,2,3,4,5,4,3,21]}, {\"name\":\"marina\", \"sorname\":\"rumyancev\", \"age\" : 25, \"numbers\" : [9,8,7,6,5,4,3,2,1]}]";

		public const int PersonsArrayCount = 2;

		#endregion

		#region Person

		public const string Person = "{\"name\":\"pavel\", \"sorname\":\"rumyancev\", \"age\" : 27}";

		public const int PersonPropertiesCount = 3;

		#endregion

		#region String

		public const string TestString = "$afddf-32wd;=jmjn";

		#endregion

		#endregion

		#region Methods

		public static string GetPerametrizedPerson(string name, string sorname, int age)
		{
			return string.Format("{{ \"name\":\"{0}\", \"surname\":\"{1}\", \"age\" : {2} }}", name, sorname, age);
		}

		#endregion
	}
}
