﻿using TMS.Common.Serialization.Json.Interpreters;

namespace TMS.Common.Serialization.Json.Api
{
	/// <summary>
	///     Exporter Function delegate
	/// </summary>
	/// <param name="obj">The obj.</param>
	/// <param name="writer">The writer.</param>
	internal delegate void ExporterFunc(object obj, JsonWriter writer);

	/// <summary>
	///     Exporter Function delegate
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="obj">The obj.</param>
	/// <param name="writer">The writer.</param>
	internal delegate void ExporterFunc<in T>(T obj, JsonWriter writer);

	/// <summary>
	///     Importer Function delegate
	/// </summary>
	/// <param name="input">The input.</param>
	/// <returns></returns>
	internal delegate object ImporterFunc(object input);

	/// <summary>
	///     Importer Function delegate
	/// </summary>
	/// <typeparam name="TJson">The type of the json.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="input">The input.</param>
	/// <returns></returns>
	public delegate TValue ImporterFunc<in TJson, out TValue>(TJson input);

	/// <summary>
	///     Wrapper Factory delegate
	/// </summary>
	/// <returns></returns>
	public delegate IJsonWrapper WrapperFactory();
}