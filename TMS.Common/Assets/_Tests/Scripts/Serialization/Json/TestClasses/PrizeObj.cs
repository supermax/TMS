#region

using System.Text;
using TMS.Common.Serialization.Json;

#endregion

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
	public class PrizeObj
	{
		[JsonDataMember(Name = "pt")]
		public string PrizeType { get; set; }

		[JsonDataMember(Name = "v")]
		public int Value { get; set; }

		public override string ToString()
		{
			var builder = new StringBuilder();

			builder.Append("PrizeType: " + PrizeType + "\n");
			builder.Append("Value: " + Value + "\n");

			return builder.ToString();
		}
	}
}