#region


#endregion

namespace TMS.Common.Serialization.Json
{
	internal interface IJsonWrapperInternal : IJsonWrapper
	{
		/// <summary>
		///     To the json.
		/// </summary>
		/// <param name="writer">The writer.</param>
		void ToJson(JsonWriter writer);
	}
}