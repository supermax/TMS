namespace TMS.Common.Network.Response.Api
{
	/// <summary>
	/// Response Payload Data Type
	/// </summary>
	public enum ResponsePayloadDataType
	{
		/// <summary>
		/// Json
		/// </summary>
		Json = 0,

		/// <summary>
		/// image
		/// </summary>
		Image,

		/// <summary>
		/// sound
		/// </summary>
		Sound,

		/// <summary>
		/// byte array
		/// </summary>
		ByteArray,

        /// <summary>
        /// asset bundle
        /// </summary>
        AssetBundle,
		
		/// <summary>
		/// file stream
		/// </summary>
		FileStream,

		/// <summary>
		/// other
		/// </summary>
		Other
	}
}