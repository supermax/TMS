#region

using System.Text;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

#endregion

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
	public class MachineObj
	{
		[JsonDataMember(Name = "id")]
		public int Id { get; set; }

		[JsonDataMember(Name = "o")]
		public int Order { get; set; }

		[JsonDataMember(Name = "n")]
		public string SlotName { get; set; }

		[JsonDataMember(Name = "v")]
		public int Version { get; set; }

		[JsonDataMember(Name = "unlockedLvl")]
		public int IsUnlocked { get; set; }

		[JsonDataMember(Name = "mt")]
		public string MachineType { get; set; }

		[JsonDataMember(Name = "br")]
		public int BonusRound { get; set; }

		[JsonDataMember(Name = "jp")]
		public int JackPot { get; set; }

		[JsonDataMember(Name = "isActive")]
		public bool IsActive { get; set; }

		[JsonDataMember(Name = "rank")]
		public float Rank { get; set; }

		[JsonDataMember(Name = "userRank")]
		public int UserRank { get; set; }

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append("Id: " + Id + "\n");
			builder.Append("Order: " + Order + "\n");
			builder.Append("SlotName: " + SlotName + "\n");
			builder.Append("Version: " + Version + "\n");
			builder.Append("IsUnlocked: " + IsUnlocked + "\n");
			builder.Append("MachineType: " + MachineType + "\n");
			builder.Append("BonusRound: " + BonusRound + "\n");
			builder.Append("JackPot: " + JackPot + "\n");
			builder.Append("IsActive: " + IsActive + "\n");
			builder.Append("Rank: " + Rank + "\n");
			builder.Append("UserRank: " + UserRank + "\n");

			return builder.ToString();
		}
	}
}