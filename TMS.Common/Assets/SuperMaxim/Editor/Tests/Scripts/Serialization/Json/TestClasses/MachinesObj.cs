#region

using System.Collections.Generic;
using System.Text;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

#endregion

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
	public class MachinesObj
	{
		[JsonDataMember(Name = "machines")]
		public List<MachineObj> Machines { get; set; }

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.Append("Machines:\n");

			foreach (var machineObj in Machines)
			{
				builder.Append(machineObj + "\n");
			}
			return builder.ToString();
		}
	}
}