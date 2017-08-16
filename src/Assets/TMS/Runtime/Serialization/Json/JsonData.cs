#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMS.Common.Core;
using TMS.Common.Extensions;
using TMS.Common.Logging;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Json Data
	/// </summary>
	public class JsonData : IJsonWrapper, IEquatable<JsonData>, IMapContainer
	{
		#region Fields

		private IList<JsonData> _instArray;
		private bool _instBoolean;
		private double _instDouble;
		private int _instInt;
		private long _instLong;
		private IDictionary<string, JsonData> _instObject;
		private string _instString;
		private string _json;

		// Used to implement the IOrderedDictionary interface
		private IList<KeyValuePair<string, JsonData>> _objectList;
		private JsonType _type;

		#endregion

		#region Properties

		/// <summary>
		///     Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		/// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection" />.</returns>
		public int Count
		{
			get { return EnsureCollection().Count; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		public bool IsArray
		{
			get { return _type == JsonType.Array; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is boolean.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is boolean; otherwise, <c>false</c>.
		/// </value>
		public bool IsBoolean
		{
			get { return _type == JsonType.Boolean; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is double.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is double; otherwise, <c>false</c>.
		/// </value>
		public bool IsDouble
		{
			get { return _type == JsonType.Double; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is int.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is int; otherwise, <c>false</c>.
		/// </value>
		public bool IsInt
		{
			get { return _type == JsonType.Int; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is long.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is long; otherwise, <c>false</c>.
		/// </value>
		public bool IsLong
		{
			get { return _type == JsonType.Long; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is object.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is object; otherwise, <c>false</c>.
		/// </value>
		public bool IsObject
		{
			get { return _type == JsonType.Object; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is string.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is string; otherwise, <c>false</c>.
		/// </value>
		public bool IsString
		{
			get { return _type == JsonType.String; }
		}

		#endregion

		#region ICollection Properties

		/// <summary>
		///     Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		/// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection" />.</returns>
		int ICollection.Count
		{
			get { return Count; }
		}

		/// <summary>
		///     Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized
		///     (thread safe).
		/// </summary>
		/// <returns>
		///     true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe);
		///     otherwise, false.
		/// </returns>
		bool ICollection.IsSynchronized
		{
			get { return EnsureCollection().IsSynchronized; }
		}

		/// <summary>
		///     Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
		object ICollection.SyncRoot
		{
			get { return EnsureCollection().SyncRoot; }
		}

		#endregion

		#region IDictionary Properties

		/// <summary>
		///     Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, false.</returns>
		bool IDictionary.IsFixedSize
		{
			get { return EnsureDictionary().IsFixedSize; }
		}

		/// <summary>
		///     Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Collections.IList" /> is read-only; otherwise, false.</returns>
		bool IDictionary.IsReadOnly
		{
			get { return EnsureDictionary().IsReadOnly; }
		}

		/// <summary>
		///     Gets an <see cref="T:System.Collections.ICollection" /> object containing the keys of the
		///     <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <returns>
		///     An <see cref="T:System.Collections.ICollection" /> object containing the keys of the
		///     <see cref="T:System.Collections.IDictionary" /> object.
		/// </returns>
		ICollection IDictionary.Keys
		{
			get
			{
				EnsureDictionary();
				var keys = _objectList.Select(entry => entry.Key).CreateList();
				return keys;
			}
		}

		/// <summary>
		///     Gets an <see cref="T:System.Collections.ICollection" /> object containing the values in the
		///     <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <returns>
		///     An <see cref="T:System.Collections.ICollection" /> object containing the values in the
		///     <see cref="T:System.Collections.IDictionary" /> object.
		/// </returns>
		ICollection IDictionary.Values
		{
			get
			{
				EnsureDictionary();
				var values = _objectList.Select(entry => entry.Value).CreateList();
				return values;
			}
		}

		#endregion

		#region IJsonWrapper Properties

		/// <summary>
		///     Gets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsArray
		{
			get { return IsArray; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is boolean.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is boolean; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsBoolean
		{
			get { return IsBoolean; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is double.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is double; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsDouble
		{
			get { return IsDouble; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is int.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is int; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsInt
		{
			get { return IsInt; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is long.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is long; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsLong
		{
			get { return IsLong; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is object.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is object; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsObject
		{
			get { return IsObject; }
		}

		/// <summary>
		///     Gets a value indicating whether this instance is string.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is string; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsString
		{
			get { return IsString; }
		}

		#endregion

		#region IMapContainer Properties

		/// <summary>
		/// Gets the map.
		/// </summary>
		/// <value>
		/// The map.
		/// </value>
		public IDictionary Map
		{
			get
			{
				var dic = EnsureDictionary();
				return dic;
			}
		}

		#endregion

		#region IList Properties

		/// <summary>
		///     Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Collections.IList" /> has a fixed size; otherwise, false.</returns>
		bool IList.IsFixedSize
		{
			get { return EnsureList().IsFixedSize; }
		}

		/// <summary>
		///     Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Collections.IList" /> is read-only; otherwise, false.</returns>
		bool IList.IsReadOnly
		{
			get { return EnsureList().IsReadOnly; }
		}

		#endregion

		#region IDictionary Indexer

		/// <summary>
		///     Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException">The key has to be a string</exception>
		object IDictionary.this[object key]
		{
			get { return EnsureDictionary()[key]; }
			set
			{
				if (!(key is String))
				{
					throw new ArgumentException("The key has to be a string");
				}

				var data = ToJsonData(value);
				this[(string)key] = data;
			}
		}

		#endregion

		#region IList Indexer

		object IList.this[int index]
		{
			get { return EnsureList()[index]; }
			set
			{
				EnsureList();
				var data = ToJsonData(value);
				this[index] = data;
			}
		}

		#endregion

#if DEBUG
		/// <summary>
		///     Gets or sets a value indicating whether [is debug mode].
		/// </summary>
		/// <remarks>DO NOT TURN ON CONSTANTLY! THIS WILL SLOW MAPER'S WORK</remarks>
		/// <value>
		///     <c>true</c> if [is debug mode]; otherwise, <c>false</c>.
		/// </value>
		public static bool IsDebugMode { get; set; }

		/// <summary>
		///     writes given message to debug log
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The arguments.</param>
		private static void Log(string format, params object[] args)
		{
			if (!IsDebugMode) return;
			var log = string.Format("[JSON_DATA] " + format, args);
			Loggers.Default.ConsoleLogger.Write(log);
		}

#endif

		#region Public Indexers

		/// <summary>
		///     Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="propName">Name of the prop.</param>
		/// <returns></returns>
		public JsonData this[string propName]
		{
			get
			{
				EnsureDictionary();

				return _instObject[propName];
			}
			set
			{
				EnsureDictionary();

				var entry = new KeyValuePair<string, JsonData>(propName, value);
				if (_instObject.ContainsKey(propName))
				{
#if DEBUG
					Log("this[{0}] contains already value: '{1}'", propName, value);
#endif
					for (var i = 0; i < _objectList.Count; i++)
					{
						if (_objectList[i].Key != propName) continue;
						_objectList[i] = entry;
						break;
					}
				}
				else
				{
					_objectList.Add(entry);
				}

				_instObject[propName] = value;
#if DEBUG
				Log("this[{0}]: _instObject[{0}] << '{1}', _instObject[propName]: {2}", propName, value, _instObject[propName]);
#endif
				_json = null;
			}
		}

		/// <summary>
		///     Gets or sets the element at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public JsonData this[int index]
		{
			get
			{
				EnsureCollection();
				return _type == JsonType.Array ? _instArray[index] : _objectList[index].Value;
			}
			set
			{
				EnsureCollection();
				if (_type == JsonType.Array)
				{
					_instArray[index] = value;
				}
				else
				{
					var entry = _objectList[index];
					var newEntry = new KeyValuePair<string, JsonData>(entry.Key, value);

					_objectList[index] = newEntry;
					_instObject[entry.Key] = value;
				}
				_json = null;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonData" /> class.
		/// </summary>
		public JsonData()
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonData" /> class.
		/// </summary>
		/// <param name="boolean">if set to <c>true</c> [boolean].</param>
		public JsonData(bool boolean)
		{
			_type = JsonType.Boolean;
			_instBoolean = boolean;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonData" /> class.
		/// </summary>
		/// <param name="number">The number.</param>
		public JsonData(double number)
		{
			_type = JsonType.Double;
			_instDouble = number;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonData" /> class.
		/// </summary>
		/// <param name="number">The number.</param>
		public JsonData(int number)
		{
			_type = JsonType.Int;
			_instInt = number;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonData" /> class.
		/// </summary>
		/// <param name="number">The number.</param>
		public JsonData(long number)
		{
			_type = JsonType.Long;
			_instLong = number;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonData" /> class.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <exception cref="System.ArgumentException">Unable to wrap the given object with JsonData</exception>
		public JsonData(object obj)
		{
			if (obj is Boolean)
			{
				_type = JsonType.Boolean;
				_instBoolean = (bool)obj;
				return;
			}

			if (obj is Double)
			{
				_type = JsonType.Double;
				_instDouble = (double)obj;
				return;
			}

			if (obj is Single)
			{
				_type = JsonType.Double;
				_instDouble = (float)obj;
				return;
			}

			if (obj is Decimal)
			{
				_type = JsonType.Double;
				_instDouble = (double)(decimal)obj;
				return;
			}

			if (obj is Int32)
			{
				_type = JsonType.Int;
				_instInt = (int)obj;
				return;
			}

			if (obj is Int16)
			{
				_type = JsonType.Int;
				_instInt = (short)obj;
				return;
			}

			if (obj is UInt16)
			{
				_type = JsonType.Int;
				_instInt = (ushort)obj;
				return;
			}

			if (obj is Byte)
			{
				_type = JsonType.Int;
				_instInt = (byte)obj;
				return;
			}

			if (obj is SByte)
			{
				_type = JsonType.Int;
				_instInt = (sbyte)obj;
				return;
			}

			if (obj is Int64)
			{
				_type = JsonType.Long;
				_instLong = (long)obj;
				return;
			}

			if (obj is UInt32)
			{
				_type = JsonType.Long;
				_instLong = (uint)obj;
				return;
			}

			_instString = obj as string;
			if (_instString != null)
			{
				_type = JsonType.String;
				return;
			}

			if (obj is char)
			{
				_type = JsonType.String;
				_instString = ((char)obj).ToString(CultureInfo.InvariantCulture);
				return;
			}

			throw new ArgumentException("Unable to wrap the given object with JsonData");
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonData" /> class.
		/// </summary>
		/// <param name="str">The STR.</param>
		public JsonData(string str)
		{
			_type = JsonType.String;
			_instString = str;
		}

		#endregion

		#region Implicit Conversions

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public static implicit operator JsonData(Boolean data)
		{
			return new JsonData(data);
		}

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public static implicit operator JsonData(Double data)
		{
			return new JsonData(data);
		}

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public static implicit operator JsonData(Int32 data)
		{
			return new JsonData(data);
		}

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public static implicit operator JsonData(Int64 data)
		{
			return new JsonData(data);
		}

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		public static implicit operator JsonData(String data)
		{
			return new JsonData(data);
		}

		#endregion

		#region Explicit Conversions

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">Instance of JsonData doesn't hold a double</exception>
		public static explicit operator Boolean(JsonData data)
		{
			if (data._type != JsonType.Boolean)
			{
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold a double");
			}
			return data._instBoolean;
		}

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">Instance of JsonData doesn't hold a double</exception>
		public static explicit operator Double(JsonData data)
		{
			if (data._type != JsonType.Double)
			{
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold a double");
			}
			return data._instDouble;
		}

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">Instance of JsonData doesn't hold an int</exception>
		public static explicit operator Int32(JsonData data)
		{
			if (data._type != JsonType.Int)
			{
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold an int");
			}
			return data._instInt;
		}

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">Instance of JsonData doesn't hold an long</exception>
		public static explicit operator Int64(JsonData data)
		{
			if (data._type != JsonType.Long)
			{
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold a long");
			}
			return data._instLong;
		}

		/// <summary>
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns></returns>
		/// <exception cref="System.InvalidCastException">Instance of JsonData doesn't hold a string</exception>
		public static explicit operator String(JsonData data)
		{
			if (data._type != JsonType.String)
			{
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold a string");
			}
			return data._instString;
		}

		#endregion

		#region ICollection Methods

		/// <summary>
		///     Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />,
		///     starting at a particular <see cref="T:System.Array" /> index.
		/// </summary>
		/// <param name="array">
		///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied
		///     from <see cref="T:System.Collections.ICollection" />. The <see cref="T:System.Array" /> must have zero-based
		///     indexing.
		/// </param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			EnsureCollection().CopyTo(array, index);
		}

		#endregion

		#region IDictionary Methods

		/// <summary>
		///     Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <param name="key">The <see cref="T:System.Object" /> to use as the key of the element to add.</param>
		/// <param name="value">The <see cref="T:System.Object" /> to use as the value of the element to add.</param>
		void IDictionary.Add(object key, object value)
		{
			var data = ToJsonData(value);

			EnsureDictionary().Add(key, data);

			var entry = new KeyValuePair<string, JsonData>((string)key, data);
			_objectList.Add(entry);

			_json = null;
		}

		/// <summary>
		///     Removes all items from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		void IDictionary.Clear()
		{
			EnsureDictionary().Clear();
			_objectList.Clear();
			_json = null;
		}

		/// <summary>
		///     Determines whether the <see cref="T:System.Collections.IDictionary" /> object contains an element with the
		///     specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary" /> object.</param>
		/// <returns>
		///     true if the <see cref="T:System.Collections.IDictionary" /> contains an element with the key; otherwise, false.
		/// </returns>
		bool IDictionary.Contains(object key)
		{
			return EnsureDictionary().Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return EnsureDictionary().GetEnumerator();
		}

		//IDictionaryEnumerator IDictionary.GetEnumerator()
		//{
		//	return ((IDictionary)this).GetEnumerator();
		//}

		/// <summary>
		///     Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		void IDictionary.Remove(object key)
		{
			EnsureDictionary().Remove(key);

			for (var i = 0; i < _objectList.Count; i++)
			{
				if (_objectList[i].Key != (string)key) continue;
				_objectList.RemoveAt(i);
				break;
			}

			_json = null;
		}

		#endregion

		#region IEnumerable Methods

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return EnsureCollection().GetEnumerator();
		}

		#endregion

		#region IJsonWrapper Methods

		/// <summary>
		///     Gets the boolean.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">JsonData instance doesn't hold a boolean</exception>
		bool IJsonWrapper.GetBoolean()
		{
			if (_type != JsonType.Boolean)
			{
				throw new InvalidOperationException(
					"JsonData instance doesn't hold a boolean");
			}
			return _instBoolean;
		}

		/// <summary>
		///     Gets the double.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">JsonData instance doesn't hold a double</exception>
		double IJsonWrapper.GetDouble()
		{
			if (_type != JsonType.Double)
			{
				throw new InvalidOperationException(
					"JsonData instance doesn't hold a double");
			}
			return _instDouble;
		}

		/// <summary>
		///     Gets the int.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">JsonData instance doesn't hold an int</exception>
		int IJsonWrapper.GetInt()
		{
			if (_type != JsonType.Int)
			{
				throw new InvalidOperationException(
					"JsonData instance doesn't hold an int");
			}
			return _instInt;
		}

		/// <summary>
		///     Gets the long.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">JsonData instance doesn't hold a long</exception>
		long IJsonWrapper.GetLong()
		{
			if (_type != JsonType.Long)
			{
				throw new InvalidOperationException(
					"JsonData instance doesn't hold a long");
			}
			return _instLong;
		}

		/// <summary>
		///     Gets the string.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">JsonData instance doesn't hold a string</exception>
		string IJsonWrapper.GetString()
		{
			if (_type != JsonType.String)
			{
				throw new InvalidOperationException(
					"JsonData instance doesn't hold a string");
			}
			return _instString;
		}

		/// <summary>
		///     Sets the boolean.
		/// </summary>
		/// <param name="val">if set to <c>true</c> [val].</param>
		void IJsonWrapper.SetBoolean(bool val)
		{
			_type = JsonType.Boolean;
			_instBoolean = val;
			_json = null;
		}

		/// <summary>
		///     Sets the double.
		/// </summary>
		/// <param name="val">The val.</param>
		void IJsonWrapper.SetDouble(double val)
		{
			_type = JsonType.Double;
			_instDouble = val;
			_json = null;
		}

		/// <summary>
		///     Sets the int.
		/// </summary>
		/// <param name="val">The val.</param>
		void IJsonWrapper.SetInt(int val)
		{
			_type = JsonType.Int;
			_instInt = val;
			_json = null;
		}

		void IJsonWrapper.SetLong(long val)
		{
			_type = JsonType.Long;
			_instLong = val;
			_json = null;
		}

		/// <summary>
		///     Sets the string.
		/// </summary>
		/// <param name="val">The val.</param>
		void IJsonWrapper.SetString(string val)
		{
			_type = JsonType.String;
			_instString = val;
			_json = null;
		}

		/// <summary>
		///     To the json.
		/// </summary>
		/// <returns></returns>
		string IJsonWrapper.ToJson()
		{
			return ToJson();
		}

		/// <summary>
		///     To the json.
		/// </summary>
		/// <param name="writer">The writer.</param>
		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			ToJson(writer);
		}

		#endregion

		#region IList Methods

		/// <summary>
		///     Adds an item to the <see cref="T:System.Collections.IList" />.
		/// </summary>
		/// <param name="value">The object to add to the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>
		///     The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the
		///     collection,
		/// </returns>
		int IList.Add(object value)
		{
			return Add(value);
		}

		/// <summary>
		///     Removes all items from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		void IList.Clear()
		{
			EnsureList().Clear();
			_json = null;
		}

		/// <summary>
		///     Determines whether the <see cref="T:System.Collections.IList" /> contains a specific value.
		/// </summary>
		/// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>
		///     true if the <see cref="T:System.Object" /> is found in the <see cref="T:System.Collections.IList" />; otherwise,
		///     false.
		/// </returns>
		bool IList.Contains(object value)
		{
			return EnsureList().Contains(value);
		}

		/// <summary>
		///     Determines the index of a specific item in the <see cref="T:System.Collections.IList" />.
		/// </summary>
		/// <param name="value">The object to locate in the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>
		///     The index of <paramref name="value" /> if found in the list; otherwise, -1.
		/// </returns>
		int IList.IndexOf(object value)
		{
			return EnsureList().IndexOf(value);
		}

		/// <summary>
		///     Inserts an item to the <see cref="T:System.Collections.IList" /> at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
		/// <param name="value">The object to insert into the <see cref="T:System.Collections.IList" />.</param>
		void IList.Insert(int index, object value)
		{
			EnsureList().Insert(index, value);
			_json = null;
		}

		/// <summary>
		///     Removes the first occurrence of a specific object from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		/// <param name="value">The object to remove from the <see cref="T:System.Collections.IList" />.</param>
		void IList.Remove(object value)
		{
			EnsureList().Remove(value);
			_json = null;
		}

		/// <summary>
		///     Removes the <see cref="T:System.Collections.IList" /> item at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index of the item to remove.</param>
		void IList.RemoveAt(int index)
		{
			EnsureList().RemoveAt(index);
			_json = null;
		}

		#endregion

		#region Private Methods

		/// <summary>
		///     Ensures the collection.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">The JsonData instance has to be initialized first</exception>
		private ICollection EnsureCollection()
		{
			switch (_type)
			{
				case JsonType.Array:
					return (ICollection)_instArray;

				case JsonType.Object:
					return (ICollection)_instObject;
			}

			throw new InvalidOperationException(
				"The JsonData instance has to be initialized first");
		}

		/// <summary>
		///     Ensures the dictionary.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="System.InvalidOperationException">Instance of JsonData is not a dictionary</exception>
		private IDictionary EnsureDictionary()
		{
			switch (_type)
			{
				case JsonType.Object:
					return (IDictionary)_instObject; // BUG?? Can be _instObject == NULL ??
			}

			if (_type != JsonType.None)
			{
				throw new InvalidOperationException(
					"Instance of JsonData is not a dictionary");
			}

			_type = JsonType.Object;
			if (_instObject == null)
			{
				_instObject = new Dictionary<string, JsonData>();
			}
			if (_objectList == null)
			{
				_objectList = new List<KeyValuePair<string, JsonData>>();
			}

			return (IDictionary)_instObject;
		}

		private IList EnsureList()
		{
			switch (_type)
			{
				case JsonType.Array:
					return (IList)_instArray;
			}

			if (_type != JsonType.None)
			{
				throw new InvalidOperationException(
					"Instance of JsonData is not a list");
			}

			_type = JsonType.Array;
			if (_instArray == null)
			{
				_instArray = new List<JsonData>();
			}

			return (IList)_instArray;
		}

		/// <summary>
		///     To the json data.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		private static JsonData ToJsonData(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			var data = obj as JsonData;
			return data ?? new JsonData(obj);
		}

		/// <summary>
		///     Writes the json.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="writer">The writer.</param>
		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj == null)
			{
				writer.Write(null);
				return;
			}

			if (obj.IsString)
			{
				writer.Write(obj.GetString());
				return;
			}

			if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
				return;
			}

			if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
				return;
			}

			if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
				return;
			}

			if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
				return;
			}

			if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (var elem in (IList)obj)
				{
					WriteJson((JsonData)elem, writer);
				}
				writer.WriteArrayEnd();
				return;
			}

			if (!obj.IsObject) return;
			writer.WriteObjectStart();

			foreach (DictionaryEntry entry in obj)
			{
				writer.WritePropertyName((string)entry.Key);
				WriteJson((JsonData)entry.Value, writer);
			}
			writer.WriteObjectEnd();
		}

		// Check equality assuming x != null and without ReferenceEqual check
		private bool EqualsImpl(JsonData x)
		{
			if (x._type != _type)
				return false;

			switch (_type)
			{
				case JsonType.None:
					return true;

				case JsonType.Object:
					if (_instObject.Count != x._instObject.Count)
						return false;

					// Json object is unordered (json.org)
					foreach (var entry in _instObject)
					{
						JsonData foundValue;
						if (!x._instObject.TryGetValue(entry.Key, out foundValue))
							return false;

						if (!Equals(entry.Value, foundValue))
							return false;
					}
					return true;

				case JsonType.Array:
					if (_instArray.Count != x._instArray.Count)
						return false;

					for (var i = 0; i < _instArray.Count; ++i)
					{
						if (!Equals(_instArray[i], x._instArray[i]))
							return false;
					}
					return true;

				case JsonType.String:
					return _instString.Equals(x._instString);

				case JsonType.Int:
					return _instInt.Equals(x._instInt);

				case JsonType.Long:
					return _instLong.Equals(x._instLong);

				case JsonType.Double:
					return _instDouble.Equals(x._instDouble);

				case JsonType.Boolean:
					return _instBoolean.Equals(x._instBoolean);
			}

			return false;
		}

		#endregion

		/// <summary>
		/// Checks equality by content
		/// </summary>
		/// <param name="x">The x.</param>
		/// <returns></returns>
		public bool Equals(JsonData x)
		{
			if (ReferenceEquals(x, null))
				return false;

			if (ReferenceEquals(this, x))
				return true;

			return EqualsImpl(x);
		}

		/// <summary>
		/// Checks equality by content
		/// </summary>
		public static bool Equals(JsonData x, JsonData y)
		{
			if (ReferenceEquals(x, y))
				return true;

			if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
				return false;

			return x.EqualsImpl(y);
		}

		/// <summary>
		///     Adds an item to the <see cref="T:System.Collections.IList" />.
		/// </summary>
		/// <param name="value">The object to add to the <see cref="T:System.Collections.IList" />.</param>
		/// <returns>
		///     The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the
		///     collection,
		/// </returns>
		public int Add(object value)
		{
			var data = ToJsonData(value);

			_json = null;

			return EnsureList().Add(data);
		}

		/// <summary>
		///     Removes all items from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		public void Clear()
		{
			if (IsObject)
			{
				((IDictionary)this).Clear();
				return;
			}

			if (IsArray)
			{
				((IList)this).Clear();
			}
		}

		/// <summary>
		///     Gets the type of the json.
		/// </summary>
		/// <returns></returns>
		public JsonType GetJsonType()
		{
			return _type;
		}

		/// <summary>
		///     Sets the type of the json.
		/// </summary>
		/// <param name="type">The type.</param>
		public void SetJsonType(JsonType type)
		{
			if (_type == type)
			{
				return;
			}
			switch (type)
			{
				case JsonType.None:
					break;

				case JsonType.Object:
					_instObject = new Dictionary<string, JsonData>();
					_objectList = new List<KeyValuePair<string, JsonData>>();
					break;

				case JsonType.Array:
					_instArray = new List<JsonData>();
					break;

				case JsonType.String:
					_instString = default(String);
					break;

				case JsonType.Int:
					_instInt = default(Int32);
					break;

				case JsonType.Long:
					_instLong = default(Int64);
					break;

				case JsonType.Double:
					_instDouble = default(Double);
					break;

				case JsonType.Boolean:
					_instBoolean = default(Boolean);
					break;
			}

			_type = type;
		}

		/// <summary>
		///     To the json.
		/// </summary>
		/// <returns></returns>
		public string ToJson()
		{
			if (_json != null)
			{
				return _json;
			}

			var sw = new StringWriter();
			var writer = new JsonWriter(sw) { Validate = false };

			WriteJson(this, writer);

			_json = sw.ToString();
			if (_json.IsNullOrEmpty())
			{
				_json = "{}"; // TODO use const + check if this legal format
			}
			_json = _json.Trim();
			if (_json.IsNullOrEmpty())
			{
				_json = "{}"; // TODO use const + check if this legal format
			}

			return _json;
		}

		/// <summary>
		///     To the json.
		/// </summary>
		/// <param name="writer">The writer.</param>
		public void ToJson(JsonWriter writer)
		{
			var oldValidate = writer.Validate;
			writer.Validate = false;
			WriteJson(this, writer);
			writer.Validate = oldValidate;
		}

		/// <summary>
		///     Return current value
		/// </summary>
		/// <returns></returns>
		public object GetPrimitiveValue()
		{
			var jType = GetJsonType();
			switch (jType)
			{
				case JsonType.Boolean:
					return _instBoolean;

				case JsonType.Double:
					return _instDouble;

				case JsonType.Int:
					return _instInt;

				case JsonType.Long:
					return _instLong;

				case JsonType.String:
					return _instString;

				default:
					return null;
			}
		}

		/// <summary>
		///     If current data is array then return it as array
		/// </summary>
		/// <returns></returns>
		public IList<JsonData> GetArray()
		{
			return _instArray;
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			switch (_type)
			{
				case JsonType.Array:
					return "JsonData array";

				case JsonType.Boolean:
					return _instBoolean.ToString();

				case JsonType.Double:
					return _instDouble.ToString(CultureInfo.InvariantCulture);

				case JsonType.Int:
					return _instInt.ToString(CultureInfo.InvariantCulture);

				case JsonType.Long:
					return _instLong.ToString(CultureInfo.InvariantCulture);

				case JsonType.Object:
					return "JsonData object";

				case JsonType.String:
					return _instString;
			}

			return "Uninitialized JsonData";
		}
	}
}
