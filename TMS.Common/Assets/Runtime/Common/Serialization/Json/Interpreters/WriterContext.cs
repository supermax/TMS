namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Writer Context
	/// </summary>
	internal class WriterContext
	{
		/// <summary>
		///     The count
		/// </summary>
		public int Count;

		/// <summary>
		///     The expecting value
		/// </summary>
		public bool ExpectingValue;

		/// <summary>
		///     The in array
		/// </summary>
		public bool InArray;

		/// <summary>
		///     The in object
		/// </summary>
		public bool InObject;

		/// <summary>
		///     The padding
		/// </summary>
		public int Padding;
	}
}