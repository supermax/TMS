#region

using System;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     JSON Data Member Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class JsonDataMemberAttribute :
#if UNITY_3D || UNITY3D
		Attribute // TODO try to inherit from serializable
#else
		Attribute
#endif
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="JsonDataMemberAttribute" /> class.
		/// </summary>
		public JsonDataMemberAttribute()
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonDataMemberAttribute" /> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public JsonDataMemberAttribute(string name)
		{
			Name = name;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonDataMemberAttribute"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="fallbackValue">The fallback value.</param>
		public JsonDataMemberAttribute(string name, object fallbackValue) : this(name)
		{
			FallbackValue = fallbackValue;
		}

		/// <summary>
		///     Gets or sets the name of the JSON field.
		/// </summary>
		/// <value>
		///     The name of JSON field.
		/// </value>
		public virtual string Name { get; set; }

		/// <summary>
		/// Gets or sets the fallback value in case of casting or derialization error.
		/// </summary>
		/// <value>
		/// The fallback value in case of error.
		/// </value>
		public virtual object FallbackValue { get; set; }

		/// <summary>
		/// Gets or sets the default value in case of serialization error.
		/// </summary>
		/// <value>
		/// The default value.
		/// </value>
		/// TODO
		public virtual object DefaultValue { get; set; }

		/// <summary>
		/// Called before serialization.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public virtual object BeforeSerialization(object src, object value)
		{
			return value; // TODO
		}

		/// <summary>
		/// Called before deserialization.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public virtual string BeforeDeserialization(object src, string value)
		{
			return value; // TODO
		}

		/// <summary>
		/// Called after serialization.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public virtual string AfterSerialization(object src, string value)
		{
			return value; // TODO
		}

		/// <summary>
		/// Called after deserialization.
		/// </summary>
		/// <param name="src">The source.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public virtual object AfterDeserialization(object src, object value)
		{
			return value; // TODO
		}
	}

	/// <summary>
	///     JSON Data Member Ignore Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class JsonDataMemberIgnoreAttribute : Attribute
	{
		
	}
}