using System.Collections.Generic;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
	[JsonDataContract]
    public class SpinResultObj : BaseDataObj
    {
        [JsonDataMember(Name = "totalWon")]
        public int TotalWon { get; set; }

        [JsonDataMember(Name = "jackPot")]
        public bool JackPot { get; set; }

        [JsonDataMember(Name = "bonusLevel")]
        public int BonusLevel { get; set; }

        [JsonDataMember(Name = "freeSpinsWon")]
        public int FreeSpinsWon { get; set; }

        [JsonDataMember(Name = "listSymbols")]
        public List<string> ListSymbols { get; set; }

        [JsonDataMember(Name = "lines")]
        public List<Line> Lines { get; set; }

        [JsonDataMember(Name = "freeSpinsResults")]
        public List<SpinResultObj> FreeSpinsResults { get; set; }

        public bool HasFreeSpins()
        {
            return FreeSpinsResults != null && FreeSpinsResults.Count > 0;
        }
    }

    [JsonDataContract]
    public class Line : BaseDataObj
    {
        [JsonDataMember(Name = "lineNumber")]
        public int LineNumber { get; set; }

        [JsonDataMember(Name = "streak")]
        public int Streak { get; set; }

        [JsonDataMember(Name = "symbolStreak")]
        public string SymbolStreak { get; set; }

        public override string ToString()
        {
            return string.Format("LineNumber = {0}, Streak = {1}, SymbolStreak = {2}", LineNumber, Streak, SymbolStreak);
        }
    }
}
