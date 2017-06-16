namespace TMS.Common.Serialization.Json
{
	/// <summary>
	/// </summary>
	internal enum Condition
	{
		/// <summary>
		///     The in array
		/// </summary>
		InArray,

		/// <summary>
		///     The in object
		/// </summary>
		InObject,

		/// <summary>
		///     The not A property
		/// </summary>
		NotAProperty,

		/// <summary>
		///     The property
		/// </summary>
		Property,

		/// <summary>
		///     The value
		/// </summary>
		Value
	}
}