#region

using System.Collections;
using System.Collections.Generic;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Ordered Dictionary Enumerator
	/// </summary>
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator
	{
		private readonly IEnumerator<KeyValuePair<string, JsonData>> _listEnumerator;

		/// <summary>
		///     Initializes a new instance of the <see cref="OrderedDictionaryEnumerator" /> class.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		public OrderedDictionaryEnumerator(
			IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			_listEnumerator = enumerator;
		}

		/// <summary>
		///     Gets the current element in the collection.
		/// </summary>
		/// <returns>The current element in the collection.</returns>
		public object Current
		{
			get { return Entry; }
		}

		/// <summary>
		///     Gets both the key and the value of the current dictionary entry.
		/// </summary>
		/// <returns>
		///     A <see cref="T:System.Collections.DictionaryEntry" /> containing both the key and the value of the current
		///     dictionary entry.
		/// </returns>
		public DictionaryEntry Entry
		{
			get
			{
				var curr = _listEnumerator.Current;
				return new DictionaryEntry(curr.Key, curr.Value);
			}
		}

		/// <summary>
		///     Gets the key of the current dictionary entry.
		/// </summary>
		/// <returns>The key of the current element of the enumeration.</returns>
		public object Key
		{
			get { return _listEnumerator.Current.Key; }
		}

		/// <summary>
		///     Gets the value of the current dictionary entry.
		/// </summary>
		/// <returns>The value of the current element of the enumeration.</returns>
		public object Value
		{
			get { return _listEnumerator.Current.Value; }
		}

		/// <summary>
		///     Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		///     true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of
		///     the collection.
		/// </returns>
		public bool MoveNext()
		{
			return _listEnumerator.MoveNext();
		}

		/// <summary>
		///     Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		public void Reset()
		{
			_listEnumerator.Reset();
		}
	}
}