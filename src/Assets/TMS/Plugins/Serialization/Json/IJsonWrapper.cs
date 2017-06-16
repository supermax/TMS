#region

using System.Collections;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Interface for Json Wrapper
	/// </summary>
	public interface IJsonWrapper : IList, IDictionary
	{
		/// <summary>
		///     Gets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		bool IsArray { get; }

		/// <summary>
		///     Gets a value indicating whether this instance is boolean.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is boolean; otherwise, <c>false</c>.
		/// </value>
		bool IsBoolean { get; }

		/// <summary>
		///     Gets a value indicating whether this instance is double.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is double; otherwise, <c>false</c>.
		/// </value>
		bool IsDouble { get; }

		/// <summary>
		///     Gets a value indicating whether this instance is int.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is int; otherwise, <c>false</c>.
		/// </value>
		bool IsInt { get; }

		/// <summary>
		///     Gets a value indicating whether this instance is long.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is long; otherwise, <c>false</c>.
		/// </value>
		bool IsLong { get; }

		/// <summary>
		///     Gets a value indicating whether this instance is object.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is object; otherwise, <c>false</c>.
		/// </value>
		bool IsObject { get; }

		/// <summary>
		///     Gets a value indicating whether this instance is string.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is string; otherwise, <c>false</c>.
		/// </value>
		bool IsString { get; }

		/// <summary>
		///     Gets the boolean.
		/// </summary>
		/// <returns></returns>
		bool GetBoolean();

		/// <summary>
		///     Gets the double.
		/// </summary>
		/// <returns></returns>
		double GetDouble();

		/// <summary>
		///     Gets the int.
		/// </summary>
		/// <returns></returns>
		int GetInt();

		/// <summary>
		///     Gets the type of the json.
		/// </summary>
		/// <returns></returns>
		JsonType GetJsonType();

		/// <summary>
		///     Gets the long.
		/// </summary>
		/// <returns></returns>
		long GetLong();

		/// <summary>
		///     Gets the string.
		/// </summary>
		/// <returns></returns>
		string GetString();

		/// <summary>
		///     Sets the boolean.
		/// </summary>
		/// <param name="val">if set to <c>true</c> [val].</param>
		void SetBoolean(bool val);

		/// <summary>
		///     Sets the double.
		/// </summary>
		/// <param name="val">The val.</param>
		void SetDouble(double val);

		/// <summary>
		///     Sets the int.
		/// </summary>
		/// <param name="val">The val.</param>
		void SetInt(int val);

		/// <summary>
		///     Sets the type of the json.
		/// </summary>
		/// <param name="type">The type.</param>
		void SetJsonType(JsonType type);

		/// <summary>
		///     Sets the long.
		/// </summary>
		/// <param name="val">The val.</param>
		void SetLong(long val);

		/// <summary>
		///     Sets the string.
		/// </summary>
		/// <param name="val">The val.</param>
		void SetString(string val);

		/// <summary>
		///     To the json.
		/// </summary>
		/// <returns></returns>
		string ToJson();

		/// <summary>
		///     To the json.
		/// </summary>
		/// <param name="writer">The writer.</param>
		void ToJson(JsonWriter writer);
	}
}