#region Code Editor
// Maxim
#endregion

using System.Collections.Generic;
using TMS.Common.Serialization.Json;

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
	public class MeRequestData
	{
		[JsonDataMember(Name = "id")]
		public string ID { get; set; }

		[JsonDataMember(Name = "name")]
		public string Name { get; set; }

		[JsonDataMember(Name = "first_name")]
		public string FirstName { get; set; }

		[JsonDataMember(Name = "last_name")]
		public string LastName { get; set; }

		[JsonDataMember(Name = "link")]
		public string Link { get; set; }

		[JsonDataMember(Name = "username")]
		public string Username { get; set; }

		[JsonDataMember(Name = "birthday")]
		public string Birthday { get; set; }

		[JsonDataMember(Name = "gender", FallbackValue = "male")]
		public string Gender { get; set; }

		[JsonDataMember(Name = "email")]
		public string Email { get; set; }

		[JsonDataMember(Name = "timezone", FallbackValue = 1, DefaultValue = 10)]
		public int Timezone { get; set; }

		[JsonDataMember(Name = "locale")]
		public string Locale { get; set; }

		[JsonDataMember(Name = "verified")]
		public bool Verified { get; set; }

		[JsonDataMember(Name = "updated_time")]
		public string UpdatedTime { get; set; }

		[JsonDataMember(Name = "favorite_athletes")]
		public IList<MeRequestData> FavoriteAthletes { get; set; }

	}
}