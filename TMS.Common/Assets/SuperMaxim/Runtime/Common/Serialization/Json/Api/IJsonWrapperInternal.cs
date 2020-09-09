#region


#endregion

using TMS.Common.Serialization.Json.Interpreters;

namespace TMS.Common.Serialization.Json.Api
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