#region

using System;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
	public class JsonDataContractAttribute : JsonDataMemberAttribute
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="JsonDataContractAttribute" /> class.
		/// </summary>
		public JsonDataContractAttribute()
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonDataContractAttribute" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public JsonDataContractAttribute(string name) : base(name)
		{
		}

		public virtual object BeforeSerialization(object value)
		{
			return value; // TODO
		}

		public virtual string BeforeDeserialization(string value)
		{
			return value; // TODO
		}

		public virtual string AfterSerialization(string value)
		{
			return value; // TODO
		}

		public virtual object AfterDeserialization(object value)
		{
			return value; // TODO
		}
	}
}