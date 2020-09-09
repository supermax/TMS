using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
    public class UserCurrencyObj : BaseDataObj
    {
        [JsonDataMember(Name = "money")]
        public int Money { get; set; }

        [JsonDataMember(Name = "currency")]
        public int Currency { get; set; }

        [JsonDataMember(Name = "lvl")]
        public int Level { get; set; }

        [JsonDataMember(Name = "xp")]
        public int Xp { get; set; }

        [JsonDataMember(Name = "fs")]
        public int FreeSpins { get; set; }
    }
}
