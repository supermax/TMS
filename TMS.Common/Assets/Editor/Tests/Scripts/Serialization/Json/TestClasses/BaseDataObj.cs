using System;
using TMS.Common.Serialization.Json;

namespace TMS.Common.Tests.Serialization.Json.TestClasses
{
    [JsonDataContract]
    public abstract class BaseDataObj
    {
        [JsonDataMember(Name = "error")]
        public string Error { get; set; }

		public bool IsErroneous()
        {
            if (Error == null)
                return false;

            return !Error.Equals(String.Empty);
        }
    }
}

