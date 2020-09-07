#define UNITY3D

#region Usings

using System;
#if !NETFX_CORE && !UNITY_METRO && !UNITY_WP8

#endif

#endregion

namespace TMS.Common.Serialization.Json.Api
{
	/// <summary>
	/// Interface for JsonMapper
	/// </summary>
	public interface IJsonMapper
	{
		/// <summary>
		///     To the json.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		string ToJson(object obj);

		/// <summary>
		///     To the object.
		/// </summary>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		JsonData ToObject(string json);

		/// <summary>
		///     To the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		T ToObject<T>(string json);

		/// <summary>
		/// To the object.
		/// </summary>
		/// <param name="json"></param>
		/// <param name="targetObjectType"></param>
		/// <returns></returns>
		object ToObject(string json, Type targetObjectType);

		/// <summary>
		///     To the object
		/// </summary>
		/// <typeparam name="T">Type to conversion</typeparam>
		/// <param name="jsonData">The json data.</param>
		/// <returns></returns>
		T ToObject<T>(JsonData jsonData);

		///// <summary>
		/////     Registers the exporter.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="exporter">The exporter.</param>
		//void RegisterExporter<T>(ExporterFunc<T> exporter);

		/// <summary>
		///     Registers the importer.
		/// </summary>
		/// <typeparam name="TJson">The type of the json.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="importer">The importer.</param>
		void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer);
	}
}
