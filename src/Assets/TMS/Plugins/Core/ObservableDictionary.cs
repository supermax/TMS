#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using TMS.Common.Extensions;

#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
using System.Runtime.Serialization;
#endif

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	///     Class ObservableDictionary
	/// </summary>
	/// <typeparam name="TKey">The type of the T key.</typeparam>
	/// <typeparam name="TValue">The type of the T value.</typeparam>
#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
	[Serializable]
#endif
	[XmlType(Namespace = "tms.com/common")]
	[XmlRoot("ObservableDictionary", Namespace = "tms.com/common", IsNullable = false)]
	public class ObservableDictionary<TKey, TValue> :
#if !UNITY3D
														Observable,
#endif
		IDictionary<TKey, TValue>,
		IDictionary,
		INotifyCollectionChanged,
		INotifyPropertyChanged
#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
, ISerializable, IDeserializationCallback
#endif
	{
		#region constructors

		#region public

		/// <summary>
		///     Initializes a new instance of the class.
		/// </summary>
		public ObservableDictionary()
		{
			_keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();
		}

		/// <summary>
		///     Initializes a new instance of the class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary)
		{
			_keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>();

			foreach (var entry in dictionary)
				DoAddEntry(entry.Key, entry.Value);
		}

		/// <summary>
		///     Initializes a new instance of the class.
		/// </summary>
		/// <param name="comparer">The comparer.</param>
		public ObservableDictionary(IEqualityComparer<TKey> comparer)
		{
			_keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);
		}

		/// <summary>
		///     Initializes a new instance of the class.
		/// </summary>
		/// <param name="dictionary">The dictionary.</param>
		/// <param name="comparer">The comparer.</param>
		public ObservableDictionary(IEnumerable<KeyValuePair<TKey, TValue>> dictionary, IEqualityComparer<TKey> comparer)
		{
			_keyedEntryCollection = new KeyedDictionaryEntryCollection<TKey>(comparer);

			foreach (var entry in dictionary)
				DoAddEntry(entry.Key, entry.Value);
		}

		#endregion public

		#region protected

#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
	/// <summary>
	///     Initializes a new instance of the class.
	/// </summary>
	/// <param name="info">The info.</param>
	/// <param name="context">The context.</param>
		protected ObservableDictionary(SerializationInfo info, StreamingContext context)
		{
			_siInfo = info;
		}
#endif

		#endregion protected

		#endregion constructors

		#region properties

		#region public

		/// <summary>
		///     Gets the comparer.
		/// </summary>
		/// <value>The comparer.</value>
		public IEqualityComparer<TKey> Comparer
		{
			get { return _keyedEntryCollection.Comparer; }
		}

		/// <summary>
		///     Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </summary>
		/// <value>The count.</value>
		/// <returns>
		///     The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </returns>
		public int Count
		{
			get { return _keyedEntryCollection.Count; }
		}

		/// <summary>
		///     Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </summary>
		/// <value>The keys.</value>
		/// <returns>
		///     An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the object that implements
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </returns>
		public Dictionary<TKey, TValue>.KeyCollection Keys
		{
			get { return TrueDictionary.Keys; }
		}

		/// <summary>
		///     Gets the error message for the property with the given name.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>`1.</returns>
		public TValue this[TKey key]
		{
			get { return (TValue) _keyedEntryCollection[key].Value; }
			set { DoSetEntry(key, value); }
		}

		/// <summary>
		///     Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </summary>
		/// <value>The values.</value>
		/// <returns>
		///     An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the object that implements
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </returns>
		public Dictionary<TKey, TValue>.ValueCollection Values
		{
			get { return TrueDictionary.Values; }
		}

		#endregion public

		#region private

		/// <summary>
		///     Gets the true dictionary.
		/// </summary>
		/// <value>The true dictionary.</value>
		protected virtual Dictionary<TKey, TValue> TrueDictionary
		{
			get
			{
				if (_dictionaryCacheVersion != _version)
				{
					_dictionaryCache.Clear();
					foreach (var entry in _keyedEntryCollection)
						_dictionaryCache.Add((TKey) entry.Key, (TValue) entry.Value);
					_dictionaryCacheVersion = _version;
				}
				return _dictionaryCache;
			}
		}

		#endregion private

		#endregion properties

		#region methods

		#region public

		/// <summary>
		///     Adds the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public void Add(TKey key, TValue value)
		{
			DoAddEntry(key, value);
		}

		/// <summary>
		///     Clears this instance.
		/// </summary>
		public void Clear()
		{
			DoClearEntries();
		}

		/// <summary>
		///     Determines whether the specified key contains key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		///     <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsKey(TKey key)
		{
			return _keyedEntryCollection.Contains(key);
		}

		/// <summary>
		///     Determines whether the specified value contains value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		///     <c>true</c> if the specified value contains value; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsValue(TValue value)
		{
			return TrueDictionary.ContainsValue(value);
		}

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return new Enumerator<TKey, TValue>(this, false);
		}

		/// <summary>
		///     Removes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		public bool Remove(TKey key)
		{
			return DoRemoveEntry(key);
		}

		/// <summary>
		///     Tries the get value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			var result = _keyedEntryCollection.Contains(key);
			value = result ? (TValue) _keyedEntryCollection[key].Value : default(TValue);
			return result;
		}

		#endregion public

		#region protected

		/// <summary>
		///     Adds the entry.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool AddEntry(TKey key, TValue value)
		{
			_keyedEntryCollection.Add(new DictionaryEntry(key, value));
			return true;
		}

		/// <summary>
		///     Clears the entries.
		/// </summary>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool ClearEntries()
		{
			// check whether there are entries to clear
			var result = (Count > 0);
			if (result)
			{
				// if so, clear the dictionary
				_keyedEntryCollection.Clear();
			}
			return result;
		}

		/// <summary>
		///     Gets the index and entry for key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="entry">The entry.</param>
		/// <returns>System.Int32.</returns>
		protected int GetIndexAndEntryForKey(TKey key, out DictionaryEntry entry)
		{
			entry = new DictionaryEntry();
			var index = -1;
			if (_keyedEntryCollection.Contains(key))
			{
				entry = _keyedEntryCollection[key];
				index = _keyedEntryCollection.IndexOf(entry);
			}
			return index;
		}

		/// <summary>
		///     Raises the event.
		/// </summary>
		/// <param name="args">
		///     The <see cref="NotifyCollectionChangedEventArgs" /> instance containing the event data.
		/// </param>
		protected
#if !UNITY3D
 async
#endif
			virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			Action act = () => { if (_collectionChanged != null) _collectionChanged.Invoke(this, args); };
#if !UNITY3D
			await
#endif
			this.SafeInvoke(act);
		}

		/// <summary>
		///     Removes the entry.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool RemoveEntry(TKey key)
		{
			// remove the entry
			return _keyedEntryCollection.Remove(key);
		}

		/// <summary>
		///     Sets the entry.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool SetEntry(TKey key, TValue value)
		{
			var keyExists = _keyedEntryCollection.Contains(key);

			// if identical key/value pair already exists, nothing to do
			if (keyExists && Equals(_keyedEntryCollection[key].Value, value))
			{
				return false;
			}

			// otherwise, remove the existing entry
			if (keyExists)
			{
				_keyedEntryCollection.Remove(key);
			}

			// add the new entry
			_keyedEntryCollection.Add(new DictionaryEntry(key, value));

			return true;
		}

		#endregion protected

		#region private

		/// <summary>
		///     Does the add entry.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		protected virtual void DoAddEntry(TKey key, TValue value)
		{
			if (!AddEntry(key, value)) return;
			_version++;

			DictionaryEntry entry;
			var index = GetIndexAndEntryForKey(key, out entry);
			FireEntryAddedNotifications(entry, index);
		}

		/// <summary>
		///     Does the clear entries.
		/// </summary>
		protected virtual void DoClearEntries()
		{
			if (!ClearEntries()) return;
			_version++;
			FireResetNotifications();
		}

		/// <summary>
		///     Does the remove entry.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		protected virtual bool DoRemoveEntry(TKey key)
		{
			DictionaryEntry entry;
			var index = GetIndexAndEntryForKey(key, out entry);

			var result = RemoveEntry(key);
			if (!result) return false;

			_version++;
			if (index > -1)
			{
				FireEntryRemovedNotifications(entry, index);
			}
			return true;
		}

		/// <summary>
		///     Does the set entry.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		protected virtual void DoSetEntry(TKey key, TValue value)
		{
			DictionaryEntry entry;
			var index = GetIndexAndEntryForKey(key, out entry);

			if (!SetEntry(key, value)) return;
			_version++;

			// if prior entry existed for this key, fire the removed notifications
			if (index > -1)
			{
				FireEntryRemovedNotifications(entry, index);

				// force the property change notifications to fire for the modified entry
				_countCache--;
			}

			// then fire the added notifications
			index = GetIndexAndEntryForKey(key, out entry);
			FireEntryAddedNotifications(entry, index);
		}

		/// <summary>
		///     Fires the entry added notifications.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <param name="index">The index.</param>
		protected virtual void FireEntryAddedNotifications(DictionaryEntry entry, int index)
		{
			// fire the relevant PropertyChanged notifications
			FirePropertyChangedNotifications();

			// fire CollectionChanged notification
			if (index > -1)
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
					new KeyValuePair<TKey, TValue>((TKey) entry.Key,
						(TValue) entry.Value), index));
			}
			else
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		/// <summary>
		///     Fires the entry removed notifications.
		/// </summary>
		/// <param name="entry">The entry.</param>
		/// <param name="index">The index.</param>
		protected virtual void FireEntryRemovedNotifications(DictionaryEntry entry, int index)
		{
			// fire the relevant PropertyChanged notifications
			FirePropertyChangedNotifications();

			// fire CollectionChanged notification
			if (index > -1)
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
					new KeyValuePair<TKey, TValue>((TKey) entry.Key,
						(TValue) entry.Value), index));
			}
			else
			{
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		/// <summary>
		///     Fires the property changed notifications.
		/// </summary>
		protected virtual void FirePropertyChangedNotifications()
		{
			if (Count == _countCache) return;
			_countCache = Count;
			OnPropertyChanged("Count");
			OnPropertyChanged("Item[]");
			OnPropertyChanged("Keys");
			OnPropertyChanged("Values");
		}

#if UNITY3D
		/// <summary>
		/// Called when [property changed].
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			ArgumentValidator.AssertNotNullOrEmpty(propertyName, "propertyName");
			PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
#endif

		/// <summary>
		///     Fires the reset notifications.
		/// </summary>
		protected virtual void FireResetNotifications()
		{
			// fire the relevant PropertyChanged notifications
			FirePropertyChangedNotifications();

			// fire CollectionChanged notification
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		#endregion private

		#endregion methods

		#region interfaces

		#region IDictionary<TKey, TValue>

		/// <summary>
		///     Adds the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			DoAddEntry(key, value);
		}

		/// <summary>
		///     Removes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			return DoRemoveEntry(key);
		}

		/// <summary>
		///     Determines whether the specified key contains key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		///     <c>true</c> if the specified key contains key; otherwise, <c>false</c>.
		/// </returns>
		bool IDictionary<TKey, TValue>.ContainsKey(TKey key)
		{
			return _keyedEntryCollection.Contains(key);
		}

		/// <summary>
		///     Tries the get value.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
		{
			return TryGetValue(key, out value);
		}

		/// <summary>
		///     Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </summary>
		/// <value>The keys.</value>
		/// <returns>
		///     An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the object that implements
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </returns>
		ICollection<TKey> IDictionary<TKey, TValue>.Keys
		{
			get { return Keys; }
		}

		/// <summary>
		///     Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </summary>
		/// <value>The values.</value>
		/// <returns>
		///     An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the object that implements
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </returns>
		ICollection<TValue> IDictionary<TKey, TValue>.Values
		{
			get { return Values; }
		}

		/// <summary>
		///     Gets the error message for the property with the given name.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>`1.</returns>
		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get { return (TValue)_keyedEntryCollection[key].Value; }
			set { DoSetEntry(key, value); }
		}

		#endregion IDictionary<TKey, TValue>

		#region IDictionary

		/// <summary>
		///     Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <param name="key">
		///     The <see cref="T:System.Object" /> to use as the key of the element to add.
		/// </param>
		/// <param name="value">
		///     The <see cref="T:System.Object" /> to use as the value of the element to add.
		/// </param>
		void IDictionary.Add(object key, object value)
		{
			DoAddEntry((TKey)key, (TValue)value);
		}

		/// <summary>
		///     Clears this instance.
		/// </summary>
		void IDictionary.Clear()
		{
			DoClearEntries();
		}

		/// <summary>
		///     Determines whether the <see cref="T:System.Collections.IDictionary" /> object contains an element with the specified key.
		/// </summary>
		/// <param name="key">
		///     The key to locate in the <see cref="T:System.Collections.IDictionary" /> object.
		/// </param>
		/// <returns>
		///     true if the <see cref="T:System.Collections.IDictionary" /> contains an element with the key; otherwise, false.
		/// </returns>
		bool IDictionary.Contains(object key)
		{
			return _keyedEntryCollection.Contains((TKey)key);
		}

		/// <summary>
		///     Returns an <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the
		///     <see
		///         cref="T:System.Collections.IDictionary" />
		///     object.
		/// </summary>
		/// <returns>
		///     An <see cref="T:System.Collections.IDictionaryEnumerator" /> object for the
		///     <see
		///         cref="T:System.Collections.IDictionary" />
		///     object.
		/// </returns>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new Enumerator<TKey, TValue>(this, true);
		}

		/// <summary>
		///     Gets a value indicating whether the <see cref="T:System.Collections.IDictionary" /> object has a fixed size.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is fixed size; otherwise, <c>false</c>.
		/// </value>
		/// <returns>
		///     true if the <see cref="T:System.Collections.IDictionary" /> object has a fixed size; otherwise, false.
		/// </returns>
		bool IDictionary.IsFixedSize
		{
			get { return false; }
		}

		/// <summary>
		///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		/// <returns>
		///     true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.
		/// </returns>
		bool IDictionary.IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		///     Gets the error message for the property with the given name.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>System.Object.</returns>
		object IDictionary.this[object key]
		{
			get { return _keyedEntryCollection[(TKey)key].Value; }
			set { DoSetEntry((TKey)key, (TValue)value); }
		}

		/// <summary>
		///     Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </summary>
		/// <value>The keys.</value>
		/// <returns>
		///     An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the object that implements
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </returns>
		ICollection IDictionary.Keys
		{
			get { return Keys; }
		}

		/// <summary>
		///     Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		void IDictionary.Remove(object key)
		{
			DoRemoveEntry((TKey)key);
		}

		/// <summary>
		///     Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </summary>
		/// <value>The values.</value>
		/// <returns>
		///     An <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the object that implements
		///     <see
		///         cref="T:System.Collections.Generic.IDictionary`2">
		///     </see>
		///     .
		/// </returns>
		ICollection IDictionary.Values
		{
			get { return Values; }
		}

		#endregion IDictionary

		#region ICollection<KeyValuePair<TKey, TValue>>

		/// <summary>
		///     Adds the specified KVP.
		/// </summary>
		/// <param name="kvp">The KVP.</param>
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> kvp)
		{
			DoAddEntry(kvp.Key, kvp.Value);
		}

		/// <summary>
		///     Clears this instance.
		/// </summary>
		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			DoClearEntries();
		}

		/// <summary>
		///     Determines whether [contains] [the specified KVP].
		/// </summary>
		/// <param name="kvp">The KVP.</param>
		/// <returns>
		///     <c>true</c> if [contains] [the specified KVP]; otherwise, <c>false</c>.
		/// </returns>
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> kvp)
		{
			return _keyedEntryCollection.Contains(kvp.Key);
		}

		/// <summary>
		///     Copies to.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="index">The index.</param>
		/// <exception cref="System.ArgumentNullException">array</exception>
		/// <exception cref="System.ArgumentOutOfRangeException">index</exception>
		/// <exception cref="System.ArgumentException">supplied array was too small</exception>
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if ((index < 0) || (index > array.Length))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if ((array.Length - index) < _keyedEntryCollection.Count)
			{
				throw new ArgumentException("supplied array was too small");
			}

			foreach (var entry in _keyedEntryCollection)
				array[index++] = new KeyValuePair<TKey, TValue>((TKey)entry.Key, (TValue)entry.Value);
		}

		/// <summary>
		///     Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </summary>
		/// <value>The count.</value>
		/// <returns>
		///     The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </returns>
		int ICollection<KeyValuePair<TKey, TValue>>.Count
		{
			get { return _keyedEntryCollection.Count; }
		}

		/// <summary>
		///     Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is read only; otherwise, <c>false</c>.
		/// </value>
		/// <returns>
		///     true if the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only; otherwise, false.
		/// </returns>
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		///     Removes the specified KVP.
		/// </summary>
		/// <param name="kvp">The KVP.</param>
		/// <returns>
		///     <c>true</c> if XXXX, <c>false</c> otherwise
		/// </returns>
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> kvp)
		{
			return DoRemoveEntry(kvp.Key);
		}

		#endregion ICollection<KeyValuePair<TKey, TValue>>

		#region ICollection

		/// <summary>
		///     Copies the elements of the <see cref="T:System.Collections.ICollection" /> to an <see cref="T:System.Array" />, starting at a particular
		///     <see
		///         cref="T:System.Array" />
		///     index.
		/// </summary>
		/// <param name="array">
		///     The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from
		///     <see
		///         cref="T:System.Collections.ICollection" />
		///     . The <see cref="T:System.Array" /> must have zero-based indexing.
		/// </param>
		/// <param name="index">
		///     The zero-based index in <paramref name="array" /> at which copying begins.
		/// </param>
		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)_keyedEntryCollection).CopyTo(array, index);
		}

		/// <summary>
		///     Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </summary>
		/// <value>The count.</value>
		/// <returns>
		///     The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
		/// </returns>
		int ICollection.Count
		{
			get { return _keyedEntryCollection.Count; }
		}

		/// <summary>
		///     Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is synchronized; otherwise, <c>false</c>.
		/// </value>
		/// <returns>
		///     true if access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.
		/// </returns>
		bool ICollection.IsSynchronized
		{
			get { return ((ICollection)_keyedEntryCollection).IsSynchronized; }
		}

		/// <summary>
		///     Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		/// <value>The sync root.</value>
		/// <returns>
		///     An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </returns>
		object ICollection.SyncRoot
		{
			get { return ((ICollection)_keyedEntryCollection).SyncRoot; }
		}

		#endregion ICollection

		#region IEnumerable<KeyValuePair<TKey, TValue>>

		/// <summary>
		///     Gets the enumerator.
		/// </summary>
		/// <returns>IEnumerator{KeyValuePair{`0`1}}.</returns>
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return new Enumerator<TKey, TValue>(this, false);
		}

		#endregion IEnumerable<KeyValuePair<TKey, TValue>>

		#region IEnumerable

		/// <summary>
		///     Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///     An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion IEnumerable

#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
		#region ISerializable
		/// <summary>
		///     Populates the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">
		///     ...
		/// </param>
		/// <param name="context">
		///     ...
		/// </param>
		/// <exception cref="System.ArgumentNullException">info</exception>
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}

			var entries = new Collection<DictionaryEntry>();
			foreach (var entry in _keyedEntryCollection)
				entries.Add(entry);
			info.AddValue("entries", entries);
		}

		#endregion ISerializable

		#region IDeserializationCallback

		/// <summary>
		///     Runs when the entire object graph has been deserialized.
		/// </summary>
		/// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
		public virtual void OnDeserialization(object sender)
		{
			if (_siInfo == null) return;
			var entries = (Collection<DictionaryEntry>)
						  _siInfo.GetValue("entries", typeof(Collection<DictionaryEntry>));
			foreach (var entry in entries)
				AddEntry((TKey)entry.Key, (TValue)entry.Value);
		}

		#endregion IDeserializationCallback
#endif

		#region INotifyCollectionChanged

		/// <summary>
		///     Occurs when the collection changes.
		/// </summary>
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add { _collectionChanged += value; }
			remove { _collectionChanged -= value; }
		}

		/// <summary>
		///     Occurs when [_collection changed].
		/// </summary>
#if !SILVERLIGHT && !NETFX_CORE
		[field: NonSerialized]
#endif
		protected virtual event NotifyCollectionChangedEventHandler _collectionChanged; // TODO use weak ref

		#endregion INotifyCollectionChanged

		#region INotifyPropertyChanged

		/// <summary>
		///     Occurs when a property value changes.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { PropertyChanged += value; }
			remove { PropertyChanged -= value; }
		}

#if UNITY3D
		protected virtual event PropertyChangedEventHandler PropertyChanged = delegate { };
#endif

		#endregion INotifyPropertyChanged

		#endregion interfaces

		#region protected classes

		#region KeyedDictionaryEntryCollection<TKey>

		/// <summary>
		///     Class KeyedDictionaryEntryCollection
		/// </summary>
		/// <typeparam name="TKeyType">The type of the T key type.</typeparam>
		protected class KeyedDictionaryEntryCollection<TKeyType> : KeyedCollection<TKeyType, DictionaryEntry>
		{
			#region constructors

			#region public

			/// <summary>
			///     Initializes a new instance of the class.
			/// </summary>
			public KeyedDictionaryEntryCollection()
			{
			}

			/// <summary>
			///     Initializes a new instance of the class.
			/// </summary>
			/// <param name="comparer">The comparer.</param>
			public KeyedDictionaryEntryCollection(IEqualityComparer<TKeyType> comparer)
#if UNITY_WEBPLAYER
				: base(comparer, 0)
#else
				: base(comparer)
#endif

			{
			}

			#endregion public

			#endregion constructors

			#region methods

			#region protected

			/// <summary>
			///     Gets the key for item.
			/// </summary>
			/// <param name="entry">The entry.</param>
			/// <returns>`0.</returns>
			protected override TKeyType GetKeyForItem(DictionaryEntry entry)
			{
				return (TKeyType)entry.Key;
			}

			#endregion protected

			#endregion methods
		}

		#endregion KeyedDictionaryEntryCollection<TKey>

		#endregion protected classes

		#region public structures

		#region Enumerator

		/// <summary>
		///     Struct Enumerator
		/// </summary>
		/// <typeparam name="TKeyType">The type of the T key type.</typeparam>
		/// <typeparam name="TValueType">The type of the T value type.</typeparam>
#if !SILVERLIGHT && !NETFX_CORE
		[Serializable, StructLayout(LayoutKind.Sequential)]
#endif
		public struct Enumerator<TKeyType, TValueType> : IEnumerator<KeyValuePair<TKeyType, TValueType>>,
														 IDictionaryEnumerator
		{
			#region constructors

			/// <summary>
			///     Initializes a new instance of the struct.
			/// </summary>
			/// <param name="dictionary">The dictionary.</param>
			/// <param name="isDictionaryEntryEnumerator">
			///     if set to <c>true</c> [is dictionary entry enumerator].
			/// </param>
			internal Enumerator(ObservableDictionary<TKeyType, TValueType> dictionary, bool isDictionaryEntryEnumerator)
			{
				_dictionary = dictionary;
				_version = dictionary._version;
				_index = -1;
				_isDictionaryEntryEnumerator = isDictionaryEntryEnumerator;
				_current = new KeyValuePair<TKeyType, TValueType>();
			}

			#endregion constructors

			#region properties

			#region public

			/// <summary>
			///     Gets the current.
			/// </summary>
			/// <value>The current.</value>
			public KeyValuePair<TKeyType, TValueType> Current
			{
				get
				{
					ValidateCurrent();
					return _current;
				}
			}

			#endregion public

			#endregion properties

			#region methods

			#region public

			/// <summary>
			///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
			/// </summary>
			public void Dispose()
			{
			}

			/// <summary>
			///     Advances the enumerator to the next element of the collection.
			/// </summary>
			/// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
			public bool MoveNext()
			{
				ValidateVersion();
				_index++;
				if (_index < _dictionary._keyedEntryCollection.Count)
				{
					_current = new KeyValuePair<TKeyType, TValueType>((TKeyType)_dictionary._keyedEntryCollection[_index].Key,
																	  (TValueType)_dictionary._keyedEntryCollection[_index].Value);
					return true;
				}
				_index = -2;
				_current = new KeyValuePair<TKeyType, TValueType>();
				return false;
			}

			#endregion public

			#region private

			/// <summary>
			///     Validates the current.
			/// </summary>
			/// <exception cref="System.InvalidOperationException">The enumerator has not been started.</exception>
			private void ValidateCurrent()
			{
				if (_index == -1)
				{
					throw new InvalidOperationException("The enumerator has not been started.");
				}
				if (_index == -2)
				{
					throw new InvalidOperationException("The enumerator has reached the end of the collection.");
				}
			}

			/// <summary>
			///     Validates the version.
			/// </summary>
			/// <exception cref="System.InvalidOperationException">The enumerator is not valid because the dictionary changed.</exception>
			private void ValidateVersion()
			{
				if (_version != _dictionary._version)
				{
					throw new InvalidOperationException("The enumerator is not valid because the dictionary changed.");
				}
			}

			#endregion private

			#endregion methods

			#region IEnumerator implementation

			/// <summary>
			///     Gets the current.
			/// </summary>
			/// <value>The current.</value>
			object IEnumerator.Current
			{
				get
				{
					ValidateCurrent();
					if (_isDictionaryEntryEnumerator)
					{
						return new DictionaryEntry(_current.Key, _current.Value);
					}
					return new KeyValuePair<TKeyType, TValueType>(_current.Key, _current.Value);
				}
			}

			/// <summary>
			///     Sets the enumerator to its initial position, which is before the first element in the collection.
			/// </summary>
			void IEnumerator.Reset()
			{
				ValidateVersion();
				_index = -1;
				_current = new KeyValuePair<TKeyType, TValueType>();
			}

			#endregion IEnumerator implemenation

			#region IDictionaryEnumerator implemenation

			/// <summary>
			///     Gets the entry.
			/// </summary>
			/// <value>The entry.</value>
			DictionaryEntry IDictionaryEnumerator.Entry
			{
				get
				{
					ValidateCurrent();
					return new DictionaryEntry(_current.Key, _current.Value);
				}
			}

			/// <summary>
			///     Gets the key.
			/// </summary>
			/// <value>The key.</value>
			object IDictionaryEnumerator.Key
			{
				get
				{
					ValidateCurrent();
					return _current.Key;
				}
			}

			/// <summary>
			///     Gets the value.
			/// </summary>
			/// <value>The value.</value>
			object IDictionaryEnumerator.Value
			{
				get
				{
					ValidateCurrent();
					return _current.Value;
				}
			}

			#endregion

			#region fields

			/// <summary>
			///     The _dictionary
			/// </summary>
			private readonly ObservableDictionary<TKeyType, TValueType> _dictionary;

			/// <summary>
			///     The _version
			/// </summary>
			private readonly int _version;

			/// <summary>
			///     The _index
			/// </summary>
			private int _index;

			/// <summary>
			///     The _current
			/// </summary>
			private KeyValuePair<TKeyType, TValueType> _current;

			/// <summary>
			///     The _is dictionary entry enumerator
			/// </summary>
			private readonly bool _isDictionaryEntryEnumerator;

			#endregion fields
		}

		#endregion Enumerator

		#endregion public structures

		#region fields

		/// <summary>
		///     The _dictionary cache
		/// </summary>
		private readonly Dictionary<TKey, TValue> _dictionaryCache = new Dictionary<TKey, TValue>();

#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
		[field: NonSerialized]
		private readonly SerializationInfo _siInfo;
#endif
		/// <summary>
		///     The _si info
		/// </summary>
		/// <summary>
		///     The _count cache
		/// </summary>
		private int _countCache;

		/// <summary>
		///     The _dictionary cache version
		/// </summary>
		private int _dictionaryCacheVersion;

		/// <summary>
		///     The _keyed entry collection
		/// </summary>
		protected KeyedDictionaryEntryCollection<TKey> _keyedEntryCollection;

		/// <summary>
		///     The _version
		/// </summary>
		private int _version;

		#endregion fields
	}
}