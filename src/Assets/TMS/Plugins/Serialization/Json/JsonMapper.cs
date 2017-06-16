#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using TMS.Common.Extensions;
using System.Diagnostics;

#if !NETFX_CORE && !UNITY_METRO && !UNITY_WP8
using System.Threading;
#endif

#endregion

namespace TMS.Common.Serialization.Json
{
	#region Interface for JsonMapper
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

		/// <summary>
		///     Registers the exporter.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="exporter">The exporter.</param>
		void RegisterExporter<T>(ExporterFunc<T> exporter);

		/// <summary>
		///     Registers the importer.
		/// </summary>
		/// <typeparam name="TJson">The type of the json.</typeparam>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="importer">The importer.</param>
		void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer);
	}
	#endregion

	/// <summary>
	///     Json Mapper
	/// </summary>
	//[IocTypeMap(typeof(IJsonMapper), true, false)]
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

		private readonly IDictionary<Type, ArrayMetadata> _arrayMetadata;
		private readonly IDictionary<Type, ExporterFunc> _baseExportersTable;
		private readonly IDictionary<Type, IDictionary<Type, ImporterFunc>> _baseImportersTable;
		private readonly IDictionary<Type, IDictionary<Type, MethodInfo>> _convOps;
		private readonly IDictionary<Type, ExporterFunc> _customExportersTable;
		private readonly IDictionary<Type, IDictionary<Type, ImporterFunc>> _customImportersTable;
		private readonly IFormatProvider _datetimeFormat;
		private readonly int _maxNestingDepth;
		private readonly IDictionary<Type, ObjectMetadata> _objectMetadata;
		private readonly IDictionary<Type, IList<PropertyMetadata>> _typeProperties;

		#region Lockers
		private readonly object _arrayMetadataLock = new object();
		private readonly object _objectMetadataLock = new object();
		private readonly object _typePropertiesLock = new object();
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
#if DEBUG
			ThrowExceptions = true;
#endif

			_maxNestingDepth = 100;
			_arrayMetadata = new Dictionary<Type, ArrayMetadata>();
			_convOps = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
			_objectMetadata = new Dictionary<Type, ObjectMetadata>();
			_typeProperties = new Dictionary<Type, IList<PropertyMetadata>>();
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
		///     Adds the array metadata.
		/// </summary>
		/// <param name="type">The type.</param>
		private void AddArrayMetadata(Type type)
		{
			if (_arrayMetadata.ContainsKey(type))
			{
				return;
			}
			var typeWrapper = type.GetTypeWrapper();
			var data = new ArrayMetadata {IsArray = typeWrapper.IsArray};

			if (typeWrapper.GetInterface(typeof(System.Collections.IList).FullName, false) != null)
			{
				data.IsList = true;
			}

			var props = typeWrapper.GetProperties();
			foreach (var pInfo in props)
			{
#if DEBUG
				Log("AddArrayMetadata(prop: {0})", pInfo);
#endif
				if (pInfo.Name != "Item")
				{
					continue;
				}

				var parameters = pInfo.GetIndexParameters();
				if (parameters.Length != 1)
				{
					continue;
				}

				if (parameters[0].ParameterType == typeof (int))
				{
					data.ElementType = pInfo.PropertyType;
				}
			}

			lock (_arrayMetadataLock)
			{
				_arrayMetadata.Add(type, data);
			}
		}

		/// <summary>
		/// Gets the data member attribute.
		/// </summary>
		/// <param name="info">The information.</param>
		/// <returns></returns>
		private static JsonDataMemberAttribute GetDataMemberAttribute(MemberInfo info)
		{
			var attributes = info.GetCustomAttributes(typeof (JsonDataMemberAttribute), true);
			var attribute = attributes.GetFirstOrDefault() as JsonDataMemberAttribute;
			return attribute ;
		}

		/// <summary>
		///     Determines whether [is ignorable member] [by the specified member information].
		/// </summary>
		/// <param name="info">The member information.</param>
		/// <returns></returns>
		private static bool IsIgnorableMember(MemberInfo info)
		{
			var attributes = info.GetCustomAttributes(typeof (JsonDataMemberIgnoreAttribute), true);
			var isIgnorable = !attributes.IsNullOrEmpty();
			return isIgnorable;
		}

		/// <summary>
		///     Adds the object metadata.
		/// </summary>
		/// <param name="type">The type.</param>
		private void AddObjectMetadata(Type type)
		{
			if (_objectMetadata.ContainsKey(type))
			{
				return;
			}
			var data = new ObjectMetadata();
			var typeWrapper = type.GetTypeWrapper();

			var dicInterfaceType = typeWrapper.GetInterface(typeof(IDictionary).FullName, false);
			if (dicInterfaceType != null)
			{
				data.IsDictionary = true;
			}

			data.Properties = new Dictionary<string, PropertyMetadata>();

			foreach (var pInfo in typeWrapper.GetProperties())
			{
				AddPropertyMetadata(pInfo, data);
			}

			foreach (var fInfo in typeWrapper.GetFields())
			{
				AddFieldMetadata(fInfo, data);
			}

			lock (_objectMetadataLock)
			{
				_objectMetadata.Add(type, data);
			}
		}

		/// <summary>
		/// Adds the field metadata.
		/// </summary>
		/// <param name="fInfo">The f information.</param>
		/// <param name="data">The data.</param>
		private void AddFieldMetadata(FieldInfo fInfo, ObjectMetadata data)
		{
			var isIgnorable = IsIgnorableMember(fInfo);
#if DEBUG
			Log("AddObjectMetadata(field: {0}, IsIngnored: {1})", fInfo, isIgnorable);
#endif
			if (isIgnorable) return;

			var attr = GetDataMemberAttribute(fInfo);
			var attrName = (attr != null ? attr.Name : null) ?? fInfo.Name;
			var fData = new PropertyMetadata {Info = fInfo, Type = fInfo.FieldType, IsField = true, Attribute = attr};
			data.Properties.Add(attrName, fData);
		}

		/// <summary>
		/// Adds the property metadata.
		/// </summary>
		/// <param name="pInfo">The p information.</param>
		/// <param name="data">The data.</param>
		private void AddPropertyMetadata(PropertyInfo pInfo, ObjectMetadata data)
		{
			var isIgnorable = IsIgnorableMember(pInfo);
#if DEBUG
			Log("AddObjectMetadata(prop: {0}, IsIngnored: {1})", pInfo, isIgnorable);
#endif
			if (isIgnorable) return;

			if (pInfo.Name == "Item")
			{
				var parameters = pInfo.GetIndexParameters();
				if (parameters.Length != 1)
				{
					return;
				}
				if (parameters[0].ParameterType == typeof (string))
				{
					data.ElementType = pInfo.PropertyType;
				}
				return;
			}

			var attr = GetDataMemberAttribute(pInfo);
			var attrName = (attr != null ? attr.Name : null) ?? pInfo.Name;
			var pData = new PropertyMetadata {Info = pInfo, Type = pInfo.PropertyType, Attribute = attr};
			data.Properties.Add(attrName, pData);
		}

		/// <summary>
		///     Adds the type properties.
		/// </summary>
		/// <param name="type">The type.</param>
		private void AddTypeProperties(Type type)
		{
			if (_typeProperties.ContainsKey(type))
			{
				return;
			}
			var typeWrapper = type.GetTypeWrapper();
#if DEBUG
			if (IsDebugMode)
			{
				Log("AddTypeProperties(typeWrapper.GetProperties())");
			}
#endif
			var propsMeta = new List<PropertyMetadata>();
			var props = typeWrapper.GetProperties();
			foreach (var pInfo in props)
			{
				if (pInfo.Name == "Item" || IsIgnorableMember(pInfo)) continue;

				propsMeta.Add(new PropertyMetadata {Info = pInfo, IsField = false});
			}
#if DEBUG
			if (IsDebugMode)
			{
				Log("AddTypeProperties(typeWrapper.GetFields())");
			}
#endif
			var fields = typeWrapper.GetFields();
			foreach (var fInfo in fields)
			{
				if (!fInfo.IsPublic || IsIgnorableMember(fInfo)) continue;

				propsMeta.Add(new PropertyMetadata { Info = fInfo, IsField = true });
			}
#if DEBUG
			if (IsDebugMode)
			{
				foreach (var prop in propsMeta)
				{
					Log("AddArrayMetadata(prop: {0})", prop);
				}
			}
#endif
			lock (_typePropertiesLock)
			{
				_typeProperties.Add(type, propsMeta);
			}
		}

		/// <summary>
		///     Gets the conv op.
		/// </summary>
		/// <param name="t1">The t1.</param>
		/// <param name="t2">The t2.</param>
		/// <returns></returns>
		private MethodInfo GetConvOp(Type t1, Type t2)
		{
			lock (_convOpsLock)
			{
				if (! _convOps.ContainsKey(t1))
				{
					_convOps.Add(t1, new Dictionary<Type, MethodInfo>());
				}
			}

			if (_convOps[t1].ContainsKey(t2))
			{
				return _convOps[t1][t2];
			}

			var typeWrapper = t1.GetTypeWrapper();
			var op = typeWrapper.GetMethod("op_Implicit", new[] {t2});
			lock (_convOpsLock)
			{
				try
				{
					_convOps[t1].Add(t2, op);
				}
				catch (ArgumentException exc) // TODO handle exc anyway
				{
#if DEBUG
					Log("Error on GetConvOp: {0}", exc);
#endif
					return _convOps[t1][t2];
				}
			}

			return op;
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
#if DEBUG
			Log("ReadObject(instType: {0}, data: {1})", instType, data);
#endif
			AddObjectMetadata(instType);

			var metadata = _objectMetadata[instType];
#if DEBUG
			Log("ReadObject(metadata: {0})", metadata);
#endif
			var instance = Activator.CreateInstance(instType);
#if DEBUG
			Log("ReadObject(instance: {0})", instance);
#endif

			foreach (KeyValuePair<string, JsonData> item in data)
			{
				if (item.Key.IsNullOrEmpty() || item.Value == null ||
					!metadata.Properties.ContainsKey(item.Key))
				{
#if DEBUG
					Log("ReadObject(inside_loop->skip_loop, key '{0}' not found or value '{1}' is null)", item.Key, item.Value);
#endif
					continue;
				}

				var propMeta = metadata.Properties[item.Key];
				var propType = propMeta.GetMemberType();
				var value = ReadValue(propType, item.Value);
#if DEBUG
				Log("ReadObject(inside_loop->ReadValue-> propMeta: {0}, propType: {1}, value: {2})", propMeta, propType, value);
#endif
				value = ConvertValue(propType, value, 
										propMeta.Attribute != null ? propMeta.Attribute.FallbackValue : null);
#if DEBUG
				Log("ReadObject(inside_loop->ConvertValue-> propMeta: {0}, propType: {1}, value: {2})", propMeta, propType, value);
#endif
				if (propMeta.IsField)
				{
					var fieldInfo = (FieldInfo) propMeta.Info;
					if (!fieldInfo.IsInitOnly) //
					{
#if NETFX_CORE || UNITY_METRO || UNITY_WP8
						fieldInfo.SetValue(instance, value);
#else
						fieldInfo.SetValue(instance, value, BindingFlags.Default, null, Thread.CurrentThread.CurrentCulture);
#endif
#if DEBUG
						Log("ReadObject(inside_loop->fieldInfo.SetValue(instance: {0}, value: {1}))", instance, value);
#endif
					}
#if DEBUG
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
#if NETFX_CORE || UNITY_METRO || UNITY_WP8
						propInfo.SetValue(instance, value, null);
#else
						propInfo.SetValue(instance, value, BindingFlags.Default, null, null, Thread.CurrentThread.CurrentCulture);
#endif
#if DEBUG
						Log("ReadObject(inside_loop->propInfo.SetValue(instance: {0}, value: {1}))", instance, value);
#endif
					}
#if DEBUG
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
#if DEBUG
			Log("ReadValue(instType: {0}, data: {1})", instType, data);
#endif
			if (!(data.IsArray || data.IsObject))
			{
#if DEBUG
				Log("ReadValue(if(!(data.IsArray || data.IsObject)) == TRUE (Primitive Value))");
#endif
				return data.GetPrimitiveValue();
			}

			if (data.IsArray)
			{
#if DEBUG
				Log("ReadValue(if (data.IsArray) == TRUE)");
#endif
				var ary = ReadArray(instType, data);
				return ary;
			}

#if DEBUG
			Log("ReadValue(instType: {0}, data: {1}, data.Count: {2}, data.JsonType: {3}) before ReadObject(...)", instType, data, data.Count, data.GetJsonType());
#endif
			var obj = ReadObject(instType, data);
			return obj;
		}

		private IList ReadArray(Type instType, JsonData data)
		{
			AddArrayMetadata(instType);
			var metadata = _arrayMetadata[instType];

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
		public object ReadValue(Type instType, JsonReader reader, object fallbackValue = null)
		{
#if DEBUG
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
			AddObjectMetadata(instType);
			var tData = _objectMetadata[instType];

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
							var v = ReadValue(propData.Type, reader,
								propData.Attribute != null ? propData.Attribute.FallbackValue : null);
#if NETFX_CORE || UNITY_METRO || UNITY_WP8
									fInfo.SetValue(instance, v);
#else
							fInfo.SetValue(instance, v, BindingFlags.Default, null, Thread.CurrentThread.CurrentCulture);
#endif
#if DEBUG
							Log("ReadValue -> {0}.SetValue({1}, {2});", fInfo, instance, v);
#endif
						}
						else
						{
							ReadValue(propData.Type, reader,
								propData.Attribute != null ? propData.Attribute.FallbackValue : null);
#if DEBUG
							Log("ReadValue -> fInfo.CanWrite = FALSE ({0})", fInfo);
#endif
						}
					}
					else
					{
						var pInfo = (PropertyInfo) propData.Info;
						if (pInfo.CanWrite)
						{
							var v = ReadValue(propData.Type, reader,
								propData.Attribute != null ? propData.Attribute.FallbackValue : null);
#if NETFX_CORE || UNITY_METRO || UNITY_WP8
									pInfo.SetValue(instance, v, null);
#else
							pInfo.SetValue(instance, v, BindingFlags.Default, null, null, Thread.CurrentThread.CurrentCulture);
#endif
#if DEBUG
							Log("ReadValue -> {0}.SetValue({1}, {2});", pInfo, instance, v);
#endif
						}
						else
						{
							ReadValue(propData.Type, reader,
								propData.Attribute != null ? propData.Attribute.FallbackValue : null);
#if DEBUG
							Log("ReadValue -> pInfo.CanWrite = FALSE ({0})", propData);
#endif
						}
					}
				}
				else
				{
					if (!tData.IsDictionary)
					{
#if DEBUG
						Log("The type {0} doesn't have the property '{1}'", instType, property);
#endif
						ReadValue(typeof (JsonData), reader, null);
					}
					else
					{
						var res = ReadValue(tData.ElementType, reader, null);
						((IDictionary) instance).Add(property, res);
#if DEBUG
						Log("ReadValue -> ((IDictionary) {0}).Add({1}, {2});", instance, property, res);
#endif
					}
				}
			}
			return instance;
		}

		private object ReadTokenInstance(Type instType, JsonReader reader)
		{
			AddArrayMetadata(instType);
			var tData = _arrayMetadata[instType];

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
#if DEBUG
			Log(format, args);
#endif
			return ThrowExceptions;
		}

#if DEBUG
		/// <summary>
		///     Gets or sets a value indicating whether [is debug mode].
		/// </summary>
		/// <remarks>DO NOT TURN ON CONSTANTLY! THIS WILL SLOW MAPER'S WORK</remarks>
		/// <value>
		///     <c>true</c> if [is debug mode]; otherwise, <c>false</c>.
		/// </value>
		public bool IsDebugMode { get; set; }

		/// <summary>
		///     writes given message to debug log
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		protected virtual void Log(string format, params object[] args)
		{
			if (!IsDebugMode) return;
			var log = string.Format("[JSON_MAPPER] " + format, args);
//#if UNITY3D || UNITY_3D
//			Debug.Log(log);
//#else
			Debug.WriteLine(log);
//#endif
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
#if DEBUG
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

#if DEBUG
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

#if DEBUG
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
						Console.Write(v);
#endif
#if DEBUG
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
		private void WriteValue(object obj, JsonWriter writer,
			bool writerIsPrivate, int depth)
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
				else
				{
					((IJsonWrapper) obj).ToJson(writer);
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
			AddTypeProperties(objType);
			var props = _typeProperties[objType];

			writer.WriteObjectStart();
			foreach (var pData in props)
			{
                // grab member name from attribute
				var attribs = pData.Info.GetCustomAttributes(typeof (JsonDataMemberAttribute), true);
				var attr = attribs.GetFirstOrDefault() as JsonDataMemberAttribute;
				var memberName = attr != null && !attr.Name.IsNullOrEmpty() ? attr.Name : pData.Info.Name;

				if (pData.IsField)
				{
					writer.WritePropertyName(memberName);
					var value = ((FieldInfo) pData.Info).GetValue(obj);
					WriteValue(value,  writer, writerIsPrivate, depth + 1);
				}
				else
				{
					var pInfo = (PropertyInfo) pData.Info;
					if (!pInfo.CanRead) continue;

					writer.WritePropertyName(memberName);

#if NETFX_CORE || UNITY_METRO || UNITY_WP8
					var value = pInfo.GetValue(obj, null);
#else
					var value = pInfo.GetValue(obj, BindingFlags.Default, null, null, Thread.CurrentThread.CurrentCulture);
#endif
					WriteValue(value, writer, writerIsPrivate, depth + 1);
				}
			}
			writer.WriteObjectEnd();
		}

		#endregion

		/// <summary>
		///     To the json.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		//public string ToJson(object obj)
		//{
		//	lock (_writerLock)
		//	{
		//		_writer.Reset();
		//		WriteValue(obj, _writer, true, 0);
		//		return _writer.ToString();
		//	}
		//}
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
		public void ToJson(object obj, JsonWriter writer)
		{
			WriteValue(obj, writer, false, 0);
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		public JsonData ToObject(JsonReader reader)
		{
			return (JsonData) ToWrapper(() => new JsonData(), reader);
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		public JsonData ToObject(TextReader reader)
		{
			var jsonReader = new JsonReader(reader);
			return (JsonData) ToWrapper(() => new JsonData(), jsonReader);
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		public JsonData ToObject(string json)
		{
#if DEBUG
			Log("ToObject(json: {0}) as JsonData", json);
#endif
			return (JsonData) ToWrapper(() => new JsonData(), json);
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		public T ToObject<T>(JsonReader reader)
		{
			return (T) ReadValue(typeof (T), reader, null);
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		public T ToObject<T>(TextReader reader)
		{
			var jsonReader = new JsonReader(reader);
			return (T) ReadValue(typeof (T), jsonReader, null);
		}

		/// <summary>
		///     To the object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="json">The json.</param>
		/// <returns></returns>
		public T ToObject<T>(string json)
		{
#if DEBUG
			Log("ToObject<{0}>(json=\"{1}\");", typeof (T), json);
#endif
			var reader = new JsonReader(json);
			return (T) ReadValue(typeof (T), reader, null);
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

#if DEBUG
			Log("ToObject(json=\"{1}\");", targetObjectType, json);
#endif
			var reader = new JsonReader(json);
			return ReadValue(targetObjectType, reader, null);
		}

		/// <summary>
		///     To the object
		/// </summary>
		/// <typeparam name="T">Type to conversion</typeparam>
		/// <param name="jsonData">The json data.</param>
		/// <returns></returns>
		public T ToObject<T>(JsonData jsonData)
		{
#if DEBUG
			Log("ToObject<{0}>(jsonData=\"{1}\");", typeof (T), jsonData);
#endif
			return (T) ReadValue(typeof (T), jsonData);
		}

		/// <summary>
		///     To the wrapper.
		/// </summary>
		/// <param name="factory">The factory.</param>
		/// <param name="reader">The reader.</param>
		/// <returns></returns>
		public IJsonWrapper ToWrapper(WrapperFactory factory,
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
		public void RegisterExporter<T>(ExporterFunc<T> exporter)
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
