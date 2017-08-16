#region
using System.Linq;
using System.Collections.Generic;
#endregion

namespace TMS.Common.Cache
{
	/// <summary>
	///     Dictionary Cache Manager
	/// </summary>
	/// <typeparam name="TKey">The type of the key.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public class DictionaryCache<TKey, TValue>
	{
		private readonly object _locker = new object();

		/// <summary>
		///     The cache items back field
		/// </summary>
		private IDictionary<TKey, TValue> _cacheItems;

		/// <summary>
		///     Gets the cache items.
		/// </summary>
		/// <value>
		///     The cache items.
		/// </value>
		public IDictionary<TKey, TValue> CacheItems
		{
			get
			{
				if (_cacheItems == null)
				{
					lock (_locker)
					{
						if (_cacheItems == null)
						{
							_cacheItems = new Dictionary<TKey, TValue>();
						}
					}
				}
				return _cacheItems;
			}
		}

		/// <summary>
		///     Tries to get the cache value
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public bool TryGetCacheValue(TKey key, out TValue value)
		{
			value = default(TValue);
			if (!CacheItems.ContainsKey(key)) return false;
			value = CacheItems[key];
			return true;
		}

		/// <summary>
		/// Tries to get the cache value.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public bool TryGetCacheValue(int index, out TValue value)
		{
			value = default(TValue);
			if (index < 0 || index >= CacheItems.Values.Count) return false;
			value = CacheItems.Values.ElementAt(index);
			return true;
		}

		/// <summary>
		///     Adds the cache item.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="overrideExisting">if set to <c>true</c> [override existing].</param>
		/// <returns></returns>
		public bool AddCacheItem(TKey key, TValue value, bool overrideExisting = true)
		{
			if (!overrideExisting && !CacheItems.ContainsKey(key)) return false;
			CacheItems[key] = value;
			return true;
		}
	}
}