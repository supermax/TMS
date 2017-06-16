namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Json Token
	/// </summary>
	public enum JsonToken
	{
		/// <summary>
		///     The none
		/// </summary>
		None,

		/// <summary>
		///     The object start
		/// </summary>
		ObjectStart,

		/// <summary>
		///     The property name
		/// </summary>
		PropertyName,

		/// <summary>
		///     The object end
		/// </summary>
		ObjectEnd,

		/// <summary>
		///     The array start
		/// </summary>
		ArrayStart,

		/// <summary>
		///     The array end
		/// </summary>
		ArrayEnd,

		/// <summary>
		///     The int
		/// </summary>
		Int,

		/// <summary>
		///     The long
		/// </summary>
		Long,

		/// <summary>
		///     The double
		/// </summary>
		Double,

		/// <summary>
		///     The string
		/// </summary>
		String,

		/// <summary>
		///     The boolean
		/// </summary>
		Boolean,

		/// <summary>
		///     The null
		/// </summary>
		Null
	}
}