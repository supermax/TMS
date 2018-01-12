#region

using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
	public class BasicUserDataObj
	{
		[JsonDataMember(Name = "money")]
		public long UserMoney { get; set; }

		[JsonDataMember(Name = "currency")]
		public int UserCurrency { get; set; }

		[JsonDataMember(Name = "fs")]
		public int FreeSpins { get; set; }

		[JsonDataMember(Name = "fullName")]
		public string FullName { get; set; }

		[JsonDataMember(Name = "img")]
		public string ImgUrl { get; set; }

		[JsonDataMember(Name = "id")]
		public string UserId { get; set; }

		[JsonDataMember(Name = "xp")]
		public long UserExperience { get; set; }

		[JsonDataMember(Name = "lvl")]
		public int UserCurrentLevel { get; set; }

		[JsonDataMember(Name = "maxLines")]
		public int MaxLines { get; set; }

		[JsonDataMember(Name = "maxBetPerLine")]
		public int MaxBetPerLine { get; set; }

		[JsonDataMember(Name = "maxBonus")]
		public int MaxBonus { get; set; }

		[JsonDataMember(Name = "level")]
		public Level Level { get; set; }
	}
}