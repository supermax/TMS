#region Usings

using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
using System;
using System.Runtime.Serialization;
#endif

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
	[Serializable]
#endif
	[XmlType(Namespace = "tms.com/common")]
	[XmlRoot("ObservableSortedDictionary", Namespace = "tms.com/common", IsNullable = false)]
	public class ObservableSortedDictionary<TKey, TValue> : ObservableDictionary<TKey, TValue>
	{
		#region constructors

		#region public

//#if UNITY3D || UNITY_3D
		/// <summary>
		/// Initializes a new instance of the <see cref="ObservableSortedDictionary{TKey, TValue}"/> class.
		/// </summary>
		public ObservableSortedDictionary()
		{
			
		}

		/// <summary>
		/// Sets the comparer.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		public void SetComparer(IComparer<DictionaryEntry> comparer)
		{
			_comparer = comparer;
		}
//#endif

		/// <summary>
		///     Initializes a new instance of the <see cref="ObservableSortedDictionary&lt;TKey, TValue&gt;" /> class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		public ObservableSortedDictionary(IComparer<DictionaryEntry> comparer)
		{
			_comparer = comparer;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="ObservableSortedDictionary&lt;TKey, TValue&gt;" /> class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		/// <param name="dictionary">The dictionary.</param>
		public ObservableSortedDictionary(IComparer<DictionaryEntry> comparer, IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
			: base(dictionary)
		{
			_comparer = comparer;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="ObservableSortedDictionary&lt;TKey, TValue&gt;" /> class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		/// <param name="equalityComparer">The equality comparer.</param>
		public ObservableSortedDictionary(IComparer<DictionaryEntry> comparer, IEqualityComparer<TKey> equalityComparer)
			: base(equalityComparer)
		{
			_comparer = comparer;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="ObservableSortedDictionary&lt;TKey, TValue&gt;" /> class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="equalityComparer">The equality comparer.</param>
		public ObservableSortedDictionary(IComparer<DictionaryEntry> comparer, IEnumerable<KeyValuePair<TKey, TValue>> dictionary,
		                                  IEqualityComparer<TKey> equalityComparer)
			: base(dictionary, equalityComparer)
		{
			_comparer = comparer;
		}

#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
	/// <summary>
	///     Initializes a new instance of the <see cref="ObservableSortedDictionary&lt;TKey, TValue&gt;" /> class.
	/// </summary>
	/// <param name="info">The info.</param>
	/// <param name="context">The context.</param>
		protected ObservableSortedDictionary(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_siInfo = info;
		}
#endif

		#endregion public

		#endregion constructors

		#region methods

		#region protected

		/// <summary>
		///     Adds the entry.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		protected override bool AddEntry(TKey key, TValue value)
		{
			var entry = new DictionaryEntry(key, value);
			var index = GetInsertionIndexForEntry(entry);
			_keyedEntryCollection.Insert(index, entry);
			return true;
		}

		/// <summary>
		///     Gets the insertion index for entry.
		/// </summary>
		/// <param name="newEntry">The new entry.</param>
		/// <returns></returns>
		protected virtual int GetInsertionIndexForEntry(DictionaryEntry newEntry)
		{
			return BinaryFindInsertionIndex(0, Count - 1, newEntry);
		}

		/// <summary>
		///     Sets the entry.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		protected override bool SetEntry(TKey key, TValue value)
		{
			var keyExists = _keyedEntryCollection.Contains(key);

			// if identical key/value pair already exists, nothing to do
			if (keyExists && value.Equals((TValue) _keyedEntryCollection[key].Value))
				return false;

			// otherwise, remove the existing entry
			if (keyExists)
				_keyedEntryCollection.Remove(key);

			// add the new entry
			var entry = new DictionaryEntry(key, value);
			var index = GetInsertionIndexForEntry(entry);
			_keyedEntryCollection.Insert(index, entry);

			return true;
		}

		#endregion protected

		#region private

		/// <summary>
		///     Binaries the index of the find insertion.
		/// </summary>
		/// <param name="first">The first.</param>
		/// <param name="last">The last.</param>
		/// <param name="entry">The entry.</param>
		/// <returns></returns>
		private int BinaryFindInsertionIndex(int first, int last, DictionaryEntry entry)
		{
			/* NOTE: old recursive code
			 if (last < first) return first;
				var mid = first + (last - first)/2;
				var result = _comparer.Compare(_keyedEntryCollection[mid], entry);
				if (result == 0) return mid;

				return result < 0
						   ? BinaryFindInsertionIndex(mid + 1, last, entry)
						   : BinaryFindInsertionIndex(first, mid - 1, entry);
			 */
			while (true)
			{
				if (last < first) return first;
				var mid = first + (last - first)/2;
				var result = _comparer.Compare(_keyedEntryCollection[mid], entry);
				if (result == 0) return mid;

				if (result < 0)
				{
					first = mid + 1;
					continue;
				}
				last = mid - 1;
			}
		}

		#endregion private

		#endregion methods

		#region interfaces

#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
		#region ISerializable

		/// <summary>
		///     Gets the object data.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <param name="context">The context.</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			if (!_comparer.GetType().IsSerializable)
			{
				throw new NotSupportedException("The supplied Comparer is not serializable.");
			}

			base.GetObjectData(info, context);
			info.AddValue("_comparer", _comparer);
		}

		#endregion ISerializable

		#region IDeserializationCallback

		/// <summary>
		///     Called when [deserialization].
		/// </summary>
		/// <param name="sender">The sender.</param>
		public override void OnDeserialization(object sender)
		{
			if (_siInfo != null)
			{
				_comparer = (IComparer<DictionaryEntry>) _siInfo.GetValue("_comparer", typeof (IComparer<DictionaryEntry>));
			}
			base.OnDeserialization(sender);
		}

		#endregion IDeserializationCallback

		[NonSerialized] private readonly SerializationInfo _siInfo;

#endif

		#endregion interfaces

		#region fields

		private IComparer<DictionaryEntry> _comparer;

		#endregion fields
	}
}