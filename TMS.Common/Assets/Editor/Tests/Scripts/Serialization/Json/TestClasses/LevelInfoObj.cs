#region

using System.Text;
using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
	public class LevelInfoObj
	{
		[JsonDataMember(Name = "ca")]
		public int ChoiceAmount { get; set; }

		[JsonDataMember(Name = "url")]
		public string Url { get; set; }

		[JsonDataMember(Name = "mb")]
		public int MoneyBonus { get; set; }

		[JsonDataMember(Name = "mbx")]
		public int MoneyBonusExtra { get; set; }

		[JsonDataMember(Name = "level")]
		public int Level { get; set; }

		[JsonDataMember(Name = "xp")]
		public int Xp { get; set; }

		[JsonDataMember(Name = "pxp")]
		public int PreviousXp { get; set; }

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append("ChoiceAmount: " + ChoiceAmount + "\n");
			builder.Append("Url: " + Url + "\n");
			builder.Append("MoneyBonus: " + MoneyBonus + "\n");
			builder.Append("MoneyBonusExtra: " + MoneyBonusExtra + "\n");
			builder.Append("Level: " + Level + "\n");
			builder.Append("Xp: " + Xp + "\n");
			builder.Append("PreviousXp: " + PreviousXp + "\n");

			return builder.ToString();
		}
	}
}