#define UNITY3D

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using TMS.Common.Extensions;
using TMS.Common.Serialization.Json.Api;
using TMS.Common.Serialization.Json.Interpreters;
using TMS.Common.Serialization.Json.Metadata;

#if DEBUG_JSON


#endif

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Json Mapper
	/// </summary>
	public class JsonMapper : IJsonMapper // : Singleton<IJsonMapper, JsonMapper>,
	{
		/// <summary>
		/// The mapper
		/// </summary>
		private static readonly JsonMapper Mapper = new JsonMapper();

		/// <summary>
		/// Gets the default.
		/// </summary>
		/// <value>
		/// The default.
		/// </value>
		public static IJsonMapper Default { get { return Mapper; }}

		#region Fields

		private readonly MetadataHandler _metadataHandler = new MetadataHandler();
		
		private readonly IDictionary<Type, ExporterFunc> _baseExportersTable;
		private readonly IDictionary<Type, IDictionary<Type, ImporterFunc>> _baseImportersTable;
		private readonly IDictionary<Type, IDictionary<Type, MethodInfo>> _convOps;
		private readonly IDictionary<Type, ExporterFunc> _customExportersTable;
		private readonly IDictionary<Type, IDictionary<Type, ImporterFunc>> _customImportersTable;
		private readonly IFormatProvider _datetimeFormat;
		private readonly int _maxNestingDepth;
		
		#region Lockers
		private readonly object _convOpsLock = new object();
		private readonly object _writerLock = new object();
		#endregion

		#endregion

		#region Constructors

		/// <summary>
		///     Initializes the <see cref="JsonMapper" /> class.
		/// </summary>
		private JsonMapper()
		{
#if DEBUG_JSON
			ThrowExceptions = true;
			IsDebugMode = true;
#endif
			_maxNestingDepth = 100;			
			_convOps = new Dictionary<Type, IDictionary<Type, MethodInfo>>();						
			_datetimeFormat = DateTimeFormatInfo.InvariantInfo;
			_baseExportersTable = new Dictionary<Type, ExporterFunc>();
			_customExportersTable = new Dictionary<Type, ExporterFunc>();
			_baseImportersTable = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
			_customImportersTable = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();

			RegisterBaseExporters();
			RegisterBaseImporters();
		}

		#endregion

		#region Private Methods

		/// <summary>
		///     Gets the conv op.
		/// </summary>
		/// <param name="t1">The t1.</param>
		/// <param name="t2">The t2.</param>
		/// <returns></returns>
		private MethodInfo GetConvOp(Type t1, Type t2)
		{
			// TODO rewrite this function for safer way
			lock (_convOpsLock)
			{
				try
				{
					MethodInfo op;
					if (_convOps.ContainsKey(t1) && _convOps[t1].ContainsKey(t2))
					{
						op = _convOps[t1][t2];
						return op;
					}

					if (!_convOps.ContainsKey(t1))
					{
						_convOps.Add(t1, new Dictionary<Type, MethodInfo>());
					}

					var typeWrapper = t1.GetTypeWrapper();
#if DEBUG_JSON
					//var publicMethods = t1.GetMethods();
#endif
					op = typeWrapper.GetMethod("op_Implicit", new[] { t2 });

					_convOps[t1].Add(t2, op);
					return op;
				}
				catch (Exception exc) // TODO handle exc anyway
				{
#if DEBUG_JSON
					Log("Error on GetConvOp: {0}", exc);
#endif
					return null;
				}
			}
		}

		private object ConvertValue(Type propType, object value, object fallbackValue)
		{
			object result = null;
			// TODO temp check, need to throw exc?
			if (value == null)
			{
				if (fallbackValue != null) result = fallbackValue;
				return result;
			}

			try
			{
				var propWrapper = propType.GetTypeWrapper();
				var valueType = value.GetType();

				if (propWrapper.IsAssignableFrom(valueType))
				{
					result = value;
				}
				else
				{
					ImporterFunc importer = null;
					if (_baseImportersTable.ContainsKey(valueType))
					{
						var importers = _baseImportersTable[valueType];
						if (importers.ContainsKey(propType))
						{
							importer = importers[propType];
						}
					}

					if (importer == null)
					{
						if (_customImportersTable.ContainsKey(valueType))
						{
							var importers = _customImportersTable[valueType];
							if (importers.ContainsKey(propType))
							{
								importer = importers[propType];
							}
						}

						if (importer == null)
						{
							Error("Cannot resolve proper value importer for property type '{0}' from value type '{1}'",
								propType, valueType);
						}
					}

					result = importer(value);
				}
			}
			catch (Exception ex)
			{
				if (fallbackValue == null)
				{
					Error("Error during value conversion: {0}", ex);
				}
				else
				{
					result = fallbackValue;
				}
			}
			return result;
		}

		private object ReadObject(Type instType, IEnumerable data)
		{
#if DEBUG_JSON
			Log("ReadObject(instType: {0}, data: {1})", instType, data);
#endif
			var metadata = _metadataHandler.AddObjectMetadata(instType);			
#if DEBUG_JSON
			Log("ReadObject(metadata: {0})", metadata);
#endif
			var instance = Activator.CreateInstance(instType);
#if DEBUG_JSON
			Log("ReadObject(instance: {0})", instance);
#endif

			foreach (KeyValuePair<string, JsonData> item in data)
			{
				if (item.Key.IsNullOrEmpty() || item.Value == null ||
					!metadata.Properties.ContainsKey(item.Key))
				{
#if DEBUG_JSON
					Log("ReadObject(inside_loop->skip_loop, key '{0}' not found or value '{1}' is null)", item.Key, item.Value);
#endif
					continue;
				}

				var propMeta = metadata.Properties[item.Key];
				var propType = propMeta.GetMemberType();
				var value = ReadValue(propType, item.Value);
#if DEBUG_JSON
				Log("ReadObject(inside_loop->ReadValue-> propMeta: {0}, propType: {1}, value: {2})", propMeta, propType, value);
#endif
				value = ConvertValue(propType, value, 
										propMeta.Attribute != null ? propMeta.Attribute.FallbackValue : null);
#if DEBUG_JSON
				Log("ReadObject(inside_loop->ConvertValue-> propMeta: {0}, propType: {1}, value: {2})", propMeta, propType, value);
#endif
				if (propMeta.IsField)
				{
					var fieldInfo = (FieldInfo) propMeta.Info;
					if (!fieldInfo.IsInitOnly) //
					{
						fieldInfo.SetValue(instance, value);
#if DEBUG_JSON
						Log("ReadObject(inside_loop->fieldInfo.SetValue(instance: {0}, value: {1}))", instance, value);
#endif
					}
#if DEBUG_JSON
					else
					{
						Log("ReadObject(inside_loop->fieldInfo.IsInitOnly == TRUE)", instance, value);
					}
#endif
				}
				else
				{
					var propInfo = (PropertyInfo) propMeta.Info;
					if (propInfo.CanWrite)
					{
						propInfo.SetValue(instance, value, null);
#if DEBUG_JSON
						Log("ReadObject(inside_loop->propInfo.SetValue(instance: {0}, value: {1}))", instance, value);
#endif
					}
#if DEBUG_JSON
					else
					{
						Log("ReadObject(inside_loop->propInfo.SetValue(instance: {0}, value: {1}))", instance, value);
					}
#endif
				}
			}

			return instance;
		}

		/// <summary>
		///     Reads the value.
		/// </summary>
		/// <param name="instType">Type of the inst.</param>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public object ReadValue(Type instType, JsonData data)
		{
#if DEBUG_JSON
			Log("ReadValue(instType: {0}, data: {1})", instType, data);
#endif
			if (!(data.IsArray || data.IsObject))
			{
#if DEBUG_JSON
				Log("ReadValue(if(!(data.IsArray || data.IsObject)) == TRUE (Primitive Value))");
#endif
				return data.GetPrimitiveValue();
			}

			if (data.IsArray)
			{
#if DEBUG_JSON
				Log("ReadValue(if (data.IsArray) == TRUE)");
#endif
				var ary = ReadArray(instType, data);
				return ary;
			}

#if DEBUG_JSON
			Log("ReadValue(instType: {0}, data: {1}, data.Count: {2}, data.JsonType: {3}) before ReadObject(...)", instType, data, data.Count, data.GetJsonType());
#endif
			var obj = ReadObject(instType, data);
			return obj;
		}

		private IList ReadArray(Type instType, JsonData data)
		{
			var metadata = _metadataHandler.AddArrayMetadata(instType);
			
			IList list;
			Type elemType;
			if (!metadata.IsArray)
			{
				list = (IList) Activator.CreateInstance(instType);
				elemType = metadata.ElementType;
			}
			else
			{
				list = new List<object>();
				elemType = instType.GetElementType();
			}

			var array = data.GetArray();
			foreach (var item in array)
			{
				var arrayItem = ReadValue(elemType, item);
				list.Add(arrayItem);
			}

			IList instance;
			if (metadata.IsArray)
			{
				var n = list.Count;
				instance = Array.CreateInstance(elemType, n);
				for (var i = 0; i < n; i++)
				{
					((Array) instance).SetValue(list[i], i);
				}
			}
			else
			{
				instance = list;
			}
			return instance;
		}

		/// <summary>
		/// Reads the value.
		/// </summary>
		/// <param name="instType">Type of the inst.</param>
		/// <param name="reader">The reader.</param>
		/// <param name="fallbackValue">The fallback value.</param>
		/// <returns></returns>
		/// <exception cref="JsonException">
		/// </exception>
		private object ReadValue(Type instType, JsonReader reader, object fallbackValue = null)
		{
#if DEBUG_JSON
			Log("ReadValue(instType: {0}, reader: {1})", instType, reader);
#endif
			reader.Read();

			var typeWrapper = instType.GetTypeWrapper();
			switch (reader.Token)
			{
				case JsonToken.ArrayEnd:
				{
					return null;
				}

				case JsonToken.Null:
					if (!typeWrapper.IsClass)
					{
						Error("Can't assign null to an instance of type {0}", instType);
					}
					return null;

				case JsonToken.Boolean:
				case JsonToken.String:
				case JsonToken.Long:
				case JsonToken.Int:
				case JsonToken.Double:
				{
					var value = ReadPrimitiveValue(instType, reader, fallbackValue, typeWrapper);
					return value;
				}
			}

			object instance = null;
			switch (reader.Token)
			{
				case JsonToken.ArrayStart:
				{
					instance = ReadTokenInstance(instType, reader);
				}
				break;

				case JsonToken.ObjectStart:
				{
					instance = ReadObjectInstance(instType, reader);
				}
				break;
			}
			return instance;
		}

		private object ReadObjectInstance(Type instType, JsonReader reader)
		{
			var tData = _metadataHandler.AddObjectMetadata(instType);			
			var instance = Activator.CreateInstance(instType);
			while (true)
			{
				reader.Read();

				if (reader.Token == JsonToken.ObjectEnd)
				{
					break;
				}

				var property = (string) reader.Value;
				ArgumentValidator.AssertNotNullOrEmpty(property, "property"); // BUG: reader.Value == NULL in some cases ???!!!

				if (tData.Properties.ContainsKey(property))
				{
					var propData = tData.Properties[property];

					if (propData.IsField)
					{
						var fInfo = (FieldInfo) propData.Info;
						if (!fInfo.IsInitOnly)
						{
							var value = ReadValue(propData.Type, reader,
								propData.Attribute != null ? propData.Attribute.FallbackValue : null);
							fInfo.SetValue(instance, value);
#if DEBUG_JSON
							Log("ReadValue -> {0}.SetValue({1}, {2});", fInfo, instance, value);
#endif
						}
						else
						{
							ReadValue(propData.Type, reader,
								propData.Attribute != null ? propData.Attribute.FallbackValue : null);
#if DEBUG_JSON
							Log("ReadValue -> fInfo.CanWrite = FALSE ({0})", fInfo);
#endif
						}
					}
					else
					{
						var pInfo = (PropertyInfo) propData.Info;
						if (pInfo.CanWrite)
						{
							var value = ReadValue(propData.Type, reader,
								propData.Attribute != null ? propData.Attribute.FallbackValue : null);
							pInfo.SetValue(instance, value, null);
#if DEBUG_JSON
							Log("ReadValue -> {0}.SetValue({1}, {2});", pInfo, instance, value);
#endif
						}
						else
						{
							ReadValue(propData.Type, reader,
								propData.Attribute != null ? propData.Attribute.FallbackValue : null);
#if DEBUG_JSON
							Log("ReadValue -> pInfo.CanWrite = FALSE ({0})", propData);
#endif
						}
					}
				}
				else
				{
					if (!tData.IsDictionary)
					{
#if DEBUG_JSON
						Log("The type {0} doesn't have the property '{1}'", instType, property);
#endif
						ReadValue(typeof (JsonData), reader, null);
					}
					else
					{
						var res = ReadValue(tData.ElementType, reader, null);
						try
						{
							((IDictionary) instance).Add(property, res);
						}
						catch(Exception exc) // TODO handle exc
						{
#if DEBUG_JSON
							Log("(({0}) instance - {1}).Add(\"{2}\", {3}); Error: {4}", typeof(IDictionary), instance, property, res, exc);
#endif
						}
#if DEBUG_JSON
						Log("ReadValue -> ((IDictionary) {0}).Add({1}, {2});", instance, property, res);
#endif
					}
				}
			}
			return instance;
		}

		private object ReadTokenInstance(Type instType, JsonReader reader)
		{
			var tData = _metadataHandler.AddArrayMetadata(instType);			
			//this.GetValueFromConverter(reader.Value, tData.ElementType, instType, )

			if (!tData.IsArray && !tData.IsList)
			{
				Error("Type {0} can't act as an array", instType);
				return null; // TODO check if need to return default(T)
			}

			IList list;
			Type elemType;

			if (!tData.IsArray)
			{
				list = (IList) Activator.CreateInstance(instType);
				elemType = tData.ElementType;
			}
			else
			{
				list = new List<object>();
				elemType = instType.GetElementType();
			}

			while (true)
			{
				var item = ReadValue(elemType, reader, null);
				if (reader.Token == JsonToken.ArrayEnd)
				{
					break;
				}
				list.Add(item);
			}

			object instance;
			if (tData.IsArray)
			{
				var n = list.Count;
				instance = Array.CreateInstance(elemType, n); // TODO ensure 'elemType' is initialized

				for (var i = 0; i < n; i++)
				{
					((Array) instance).SetValue(list[i], i);
				}
			}
			else
			{
				instance = list;
			}
			return instance;
		}

		private bool GetValueFromConverter(object srcValue, Type jsonType, Type instType, out object value)
		{
			value = null;

			// If there's a custom importer that fits, use it
			if (_customImportersTable.ContainsKey(jsonType) &&
				_customImportersTable[jsonType].ContainsKey(instType))
			{
				var importer = _customImportersTable[jsonType][instType];
				value = importer(srcValue);
				return true;
			}

			// Maybe there's a base importer that works
			if (_baseImportersTable.ContainsKey(jsonType) &&
				_baseImportersTable[jsonType].ContainsKey(instType))
			{
				var importer = _baseImportersTable[jsonType][instType];
				value = importer(srcValue);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Reads the primitive value.
		/// </summary>
		/// <param name="instType">Type of the inst.</param>
		/// <param name="reader">The reader.</param>
		/// <param name="fallbackValue">The fallback value.</param>
		/// <param name="typeWrapper">The type wrapper.</param>
		/// <returns></returns>
		/// <exception cref="JsonException"></exception>
		private object ReadPrimitiveValue(Type instType, JsonReader reader, object fallbackValue, ITypeWrapper typeWrapper)
		{
			var jsonType = reader.Value.GetType();
			if (typeWrapper.IsAssignableFrom(jsonType))
			{
				return reader.Value;
			}

			// If there's an importer that fits, use it
			object value;
			if (GetValueFromConverter(reader.Value, jsonType, instType, out value))
			{
				return value;
			}

			// Maybe it's an enum
			if (instType.GetTypeWrapper().IsEnum)
			{
				return Enum.ToObject(instType, reader.Value);
			}

			// Try using an implicit conversion operator
			var convOp = GetConvOp(instType, jsonType);
			if (convOp != null)
			{
				return convOp.Invoke(null, new[] {reader.Value});
			}

			if (fallbackValue != null) return fallbackValue;

			// No luck
			Error("Can't assign value '{0}' (type {1}) to type {2}",
						reader.Value, jsonType, instType);
			return value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether [throw exceptions].
		/// </summary>
		/// <value>
		///   <c>true</c> if [throw exceptions]; otherwise, <c>false</c>.
		/// </value>
		public bool ThrowExceptions { get; set; }

		/// <summary>
		/// Throws exception or prints error log the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		/// <exception cref="JsonException"></exception>
		protected virtual bool Error(string format, params object[] args)
		{
			var msg = string.Format(format, args);
			if (ThrowExceptions)
			{
				throw new JsonException(msg);
			}
#if DEBUG_JSON
			Log(format, args);
#endif
			return ThrowExceptions;
		}

#if DEBUG_JSON
		/// <summary>
		///     Gets or sets a value indicating whether [is debug mode].
		/// </summary>
		/// <remarks>DO NOT TURN ON CONSTANTLY! THIS WILL SLOW MAPER'S WORK</remarks>
		/// <value>
		///     <c>true</c> if [is debug mode]; otherwise, <c>false</c>.
		/// </value>
		public bool IsDebugMode 
		{ 
			get { return _metadataHandler.IsDebugMode; }
			set { _metadataHandler.IsDebugMode = value; }
		}

		/// <summary>
		///     writes given message to debug log
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		protected virtual void Log(string format, params object[] args)
		{
			_metadataHandler.Log(format, args);
		}
#endif

		/// <summary>
		///     Reads the value.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		private IJsonWrapper ReadValue(WrapperFactory factory, JsonReader reader)
		{
#if DEBUG_JSON
			Log("ReadValue(factory: {0}, reader: {1})", factory, reader);
#endif

			reader.Read();

			switch (reader.Token)
			{
				case JsonToken.Null:
				case JsonToken.ArrayEnd:
					return null;
			}

			var instance = factory();

#if DEBUG_JSON
			Log("ReadValue(factoryInstance: {0}, reader: {1}, token: {2})", factory, reader, reader.Token);
#endif

			switch (reader.Token)
			{
				case JsonToken.String:
					instance.SetString((string) reader.Value);
					return instance;

				case JsonToken.Double:
					instance.SetDouble((double) reader.Value);
					return instance;

				case JsonToken.Int:
					instance.SetInt((int) reader.Value);
					return instance;

				case JsonToken.Long:
					instance.SetLong((long) reader.Value);
					return instance;

				case JsonToken.Boolean:
					instance.SetBoolean((bool) reader.Value);
					return instance;

				case JsonToken.ArrayStart:
					instance.SetJsonType(JsonType.Array);
					while (true)
					{
						var item = ReadValue(factory, reader);
						if (reader.Token == JsonToken.ArrayEnd)
						{
							break;
						}
						instance.Add(item);

#if DEBUG_JSON
						Log("ReadValue(factoryInstance: {0}, reader: {1}, token: {2}, item: {3})", factory, reader, reader.Token, item);
#endif
					}
					break;

				case JsonToken.ObjectStart:
					instance.SetJsonType(JsonType.Object);
					while (true)
					{
						reader.Read();
						if (reader.Token == JsonToken.ObjectEnd)
						{
							break;
						}

						var property = (string) reader.Value;
						var val = ReadValue(factory, reader);
						instance[property] = val;

#if UNITY3D || UNITY_3D
						// TODO | BUG -> TEMP CODE. DO NOT REMOVE!!! DESERIALIZATION IN iOS WILL NOT WORK!!!
						var v = instance[property];
						System.Diagnostics.Debug.WriteLine(v);
#endif
#if DEBUG_JSON
						Log("ReadValue(factoryInstance: {0}, reader: {1}, token: {2}, key: {3}, value: {4}, instance[property]: {5}, instance[property] == value: {6}, val.JsonType: {7})",
								factory, reader, reader.Token, property, val, instance[property], instance[property] == val,
								val != null ? val.GetJsonType() : JsonType.None);
#endif
					}
					break;
			}

			return instance;
		}

		/// <summary>
		///     Registers the base exporters.
		/// </summary>
		private void RegisterBaseExporters()
		{
			_baseExportersTable[typeof (byte)] =
				(obj, writer) => writer.Write(Convert.ToInt32((byte) obj));

			_baseExportersTable[typeof (char)] =
				(obj, writer) => writer.Write(Convert.ToString((char) obj));

			_baseExportersTable[typeof (DateTime)] =
				(obj, writer) => writer.Write(Convert.ToString((DateTime) obj,
					_datetimeFormat));

			_baseExportersTable[typeof(TimeSpan)] =
				(obj, writer) => writer.Write(Convert.ToString((TimeSpan)obj,
					_datetimeFormat));

			_baseExportersTable[typeof (decimal)] =
				(obj, writer) => writer.Write((decimal) obj);

			_baseExportersTable[typeof (sbyte)] =
				(obj, writer) => writer.Write(Convert.ToInt32((sbyte) obj));

			_baseExportersTable[typeof (short)] =
				(obj, writer) => writer.Write(Convert.ToInt32((short) obj));

			_baseExportersTable[typeof (ushort)] =
				(obj, writer) => writer.Write(Convert.ToInt32((ushort) obj));

			_baseExportersTable[typeof (uint)] =
				(obj, writer) => writer.Write(Convert.ToUInt64((uint) obj));

			_baseExportersTable[typeof (ulong)] =
				(obj, writer) => writer.Write((ulong) obj);
		}

		/// <summary>
		///     Registers the base importers.
		/// </summary>
		private void RegisterBaseImporters()
		{
			ImporterFunc importer = input => Convert.ToByte((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (byte), importer);

			importer = input => Convert.ToInt64((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (long), importer);

			importer = input => Convert.ToUInt64((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (ulong), importer);

			importer = input => Convert.ToSByte((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (sbyte), importer);

			importer = input => Convert.ToInt16((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (short), importer);

			importer = input => Convert.ToUInt16((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (ushort), importer);

			importer = input => Convert.ToUInt32((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (uint), importer);

			importer = input => Convert.ToSingle((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (float), importer);

			importer = input => Convert.ToDouble((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (double), importer);

			importer = input => Convert.ToDouble((float) input);
			RegisterImporter(_baseImportersTable, typeof (float),
				typeof (double), importer);

			importer = input => Convert.ToString((int) input);
			RegisterImporter(_baseImportersTable, typeof (int),
				typeof (string), importer);

			importer = input => Convert.ToDecimal((double) input);
			RegisterImporter(_baseImportersTable, typeof (double),
				typeof (decimal), importer);

			importer = input => Convert.ToSingle((double) input);
			RegisterImporter(_baseImportersTable, typeof (double),
				typeof (float), importer);

			importer = input => Convert.ToUInt32((long) input);
			RegisterImporter(_baseImportersTable, typeof (long),
				typeof (uint), importer);

			importer = input => Convert.ToChar((string) input);
			RegisterImporter(_baseImportersTable, typeof (string),
				typeof (char), importer);

			importer = input => Convert.ToDateTime((string) input, _datetimeFormat);
			RegisterImporter(_baseImportersTable, typeof (string),
				typeof (DateTime), importer);

			importer = input => Convert.ToDateTime((string) input, _datetimeFormat);
			RegisterImporter(_baseImportersTable, typeof (string),
				typeof (TimeSpan), importer);

			importer = input => Convert.ToSingle((int)input);
			RegisterImporter(_baseImportersTable, typeof(int),
				typeof(float), importer);

			importer = input => Convert.ToSingle((int)input);
			RegisterImporter(_baseImportersTable, typeof(int),
				typeof(float?), importer);
		}

		/// <summary>
		///     Registers the importer.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="jsonType">Type of the json.</param>
		/// <param name="valueType">Type of the value.</param>
		/// <param name="importer">The importer.</param>
		private static void RegisterImporter(
			IDictionary<Type, IDictionary<Type, ImporterFunc>> table,
			Type jsonType, Type valueType, ImporterFunc importer)
		{
			if (! table.ContainsKey(jsonType))
			{
				table.Add(jsonType, new Dictionary<Type, ImporterFunc>());
			}

			table[jsonType][valueType] = importer;
		}

		/// <summary>
		///     Writes the value.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="writer">The writer.</param>
		/// <param name="writerIsPrivate">if set to <c>true</c> [writer is private].</param>
		/// <param name="depth">The depth.</param>
		/// <exception cref="JsonException"></exception>
		private void WriteValue(object obj, JsonWriter writer, bool writerIsPrivate, int depth)
		{
			if (depth > _maxNestingDepth)
			{
				Error("Max allowed object depth reached while " +
								  "trying to export from type {0}", obj.GetType());
				return;
			}

			if (obj == null)
			{
				writer.Write(null);
				return;
			}

			if (obj is IJsonWrapper)
			{
				if (writerIsPrivate)
				{
					writer.WriteRawValue(((IJsonWrapper) obj).ToJson());
				}
				else if(obj is IJsonWrapperInternal)
				{
					((IJsonWrapperInternal) obj).ToJson(writer);
				}
				return;
			}

			if (obj is string)
			{
				writer.Write((string) obj);
				return;
			}

			if (obj is double)
			{
				writer.Write((double) obj);
				return;
			}

			if (obj is float)
			{
				writer.Write((float)obj);
				return;
			}

			if (obj is int)
			{
				writer.Write((int) obj);
				return;
			}

			if (obj is bool)
			{
				writer.Write((bool) obj);
				return;
			}

			if (obj is Int64)
			{
				writer.Write((long) obj);
				return;
			}

			if (obj is Array)
			{
				writer.WriteArrayStart();
				foreach (var elem in (Array) obj)
				{
					WriteValue(elem, writer, writerIsPrivate, depth + 1);
				}
				writer.WriteArrayEnd();
				return;
			}

			if (obj is IList)
			{
				writer.WriteArrayStart();
				foreach (var elem in (IList) obj)
				{
					WriteValue(elem, writer, writerIsPrivate, depth + 1);
				}
				writer.WriteArrayEnd();
				return;
			}

			if (obj is IDictionary)
			{
				writer.WriteObjectStart();
				foreach (DictionaryEntry entry in (IDictionary) obj)
				{
					writer.WritePropertyName((string) entry.Key);
					WriteValue(entry.Value, writer, writerIsPrivate, depth + 1);
				}
				writer.WriteObjectEnd();

				return;
			}

			var objType = obj.GetType();

			// See if there's a custom exporter for the object
			if (_customExportersTable.ContainsKey(objType))
			{
				var exporter = _customExportersTable[objType];
				exporter(obj, writer);
				return;
			}

			// If not, maybe there's a base exporter
			if (_baseExportersTable.ContainsKey(objType))
			{
				var exporter = _baseExportersTable[objType];
				exporter(obj, writer);
				return;
			}

			// Last option, let's see if it's an enum
			if (obj is Enum)
			{
				var eType = Enum.GetUnderlyingType(objType);
				if (eType == typeof (long) || eType == typeof (uint) || eType == typeof (ulong))
				{
					writer.Write((ulong) obj); // TODO fix cast
				}
				else
				{
					writer.Write((int) obj); // TODO fix cast
				}

				return;
			}

			// Okay, so it looks like the input should be exported as an object
			var props = _metadataHandler.AddTypeProperties(objType);
			
			writer.WriteObjectStart();
			foreach (var pData in props)
			{     				
				if (pData.IsField)
				{
					writer.WritePropertyName(pData.MemberName);
					var value = ((FieldInfo) pData.Info).GetValue(obj);
					WriteValue(value,  writer, writerIsPrivate, depth + 1);
				}
				else
				{
					var pInfo = (PropertyInfo) pData.Info;
					if (!pInfo.CanRead) continue;

					writer.WritePropertyName(pData.MemberName);

					var value = pInfo.GetValue(obj, null);
					WriteValue(value, writer, writerIsPrivate, depth + 1);
				}
			}
			writer.WriteObjectEnd();
		}

		#endregion

		public string ToJson(object obj)
		{
			var jd = obj as JsonData;
			if (jd != null)
			{
				var json = jd.ToJson();
				return json;
			}

			string result;
			lock (_writerLock)
			{
				using (var writer = new JsonWriter())
				{
					WriteValue(obj, writer, true, 0);
					result = writer.ToString();
				}
			}
			return result;
		}

		/// <summary>
		///     To the json.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="writer">The writer.</param>
		internal void ToJson(object obj, JsonWriter writer)
		{
			WriteValue(obj, writer, false, 0);
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		internal JsonData ToObject(JsonReader reader)
		{
			var result = (JsonData) ToWrapper(() => new JsonData(), reader);
			return result;
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		internal JsonData ToObject(TextReader reader)
		{
			var jsonReader = new JsonReader(reader);
			var result = (JsonData) ToWrapper(() => new JsonData(), jsonReader);
			return result;
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		public JsonData ToObject(string json)
		{
#if DEBUG_JSON
			Log("ToObject(json: {0}) as JsonData", json);
#endif
			var result = (JsonData) ToWrapper(() => new JsonData(), json);
			return result;
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		internal T ToObject<T>(JsonReader reader)
		{
			var result = (T) ReadValue(typeof (T), reader, null);
			return result;
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		internal T ToObject<T>(TextReader reader)
		{
			var jsonReader = new JsonReader(reader);
			var result = (T) ReadValue(typeof (T), jsonReader, null);
			return result;
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		public T ToObject<T>(string json)
		{
#if DEBUG_JSON
			Log("ToObject<{0}>(json=\"{1}\");", typeof (T), json);
#endif
			var reader = new JsonReader(json);
			var result = (T) ReadValue(typeof (T), reader, null);
			return result;
		}
		/// <summary>
		/// To the object
		/// </summary>
		/// <param name="json"></param>
		/// <param name="targetObjectType"></param>
		/// <returns></returns>
		public object ToObject(string json, Type targetObjectType)
		{
			if (targetObjectType == typeof (JsonData))
			{
				var res = ToObject(json);
				return res;
			}

#if DEBUG_JSON
			Log("ToObject(json=\"{1}\");", targetObjectType, json);
#endif
			var reader = new JsonReader(json);
			var result = ReadValue(targetObjectType, reader, null);
			return result;
		}

		/// <summary>
		///     To the object
		/// </summary>
		/// <typeparam name="T">Type to conversion</typeparam>
		/// <param name="jsonData">The json data.</param>
		/// <returns></returns>
		public T ToObject<T>(JsonData jsonData)
		{
#if DEBUG_JSON
			Log("ToObject<{0}>(jsonData=\"{1}\");", typeof (T), jsonData);
#endif
			var result = (T) ReadValue(typeof (T), jsonData);
			return result;
		}

		/// <summary>
		///     To the wrapper.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		internal IJsonWrapper ToWrapper(WrapperFactory factory,
			JsonReader reader)
		{
			return ReadValue(factory, reader);
		}

		/// <summary>
		///     To the wrapper.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		public IJsonWrapper ToWrapper(WrapperFactory factory, string json)
		{
			var reader = new JsonReader(json);
			return ReadValue(factory, reader);
		}

		/// <summary>
		///     Registers the exporter.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="exporter">The exporter.</param>
		internal void RegisterExporter<T>(ExporterFunc<T> exporter)
		{
			ExporterFunc exporterWrapper = (obj, writer) => exporter((T) obj, writer);
			_customExportersTable[typeof (T)] = exporterWrapper;
		}

		/// <summary>
		///     Registers the importer.
		/// </summary>
		/// <typeparam name="TJson">The type of the json.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="importer">The importer.</param>
		public void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer)
		{
			ImporterFunc importerWrapper = input => importer((TJson) input);
			RegisterImporter(_customImportersTable, typeof (TJson), typeof (TValue), importerWrapper);
		}

		/// <summary>
		///     Unregisters the exporters.
		/// </summary>
		public void UnregisterExporters()
		{
			_customExportersTable.Clear();
		}

		/// <summary>
		///     Unregisters the importers.
		/// </summary>
		public void UnregisterImporters()
		{
			_customImportersTable.Clear();
		}	    
	}
}
