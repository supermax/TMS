using System;
using TMS.Common.Cache;
using TMS.Common.Extensions;

namespace TMS.Common.Tasks.Delay
{
	/// <summary>
	/// Task Delay Manager
	/// </summary>
	public class TaskDelayManager : IDisposable
	{
		private readonly DictionaryCache<Guid, TaskDelayItem> _items = new DictionaryCache<Guid, TaskDelayItem>();

		private readonly object _locker = new object();

		/// <summary>
		/// Gets a value indicating whether [is aborted].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is aborted]; otherwise, <c>false</c>.
		/// </value>
		public bool IsAborted { get; private set; }

		/// <summary>
		/// Gets a value indicating whether [is disposed].
		/// </summary>
		/// <value>
		///   <c>true</c> if [is disposed]; otherwise, <c>false</c>.
		/// </value>
		public bool IsDisposed { get; private set; }

		/// <summary>
		/// Aborts all.
		/// </summary>
		public void AbortAll()
		{
			if (IsDisposed)
			{
				// TODO throw exception?
				return;
			}

			IsAborted = true;
			lock (_locker)
			{
				_items.CacheItems.ForEach(item => item.Value.Dispose());
				_items.CacheItems.Clear();	
			}
		}

		/// <summary>
		/// Delays the specified span.
		/// </summary>
		/// <param name="span">The span.</param>
		/// <returns></returns>
		public TaskDelayItem Delay(TimeSpan span)
		{
			if (IsDisposed)
			{
				// TODO throw exception?
				return null;
			}

			using(var item = new TaskDelayItem(Guid.NewGuid()))
			{
				_items.AddCacheItem(item.Id, item);
				
				item.Delay(span);
				
				if (_items.CacheItems.ContainsKey(item.Id))
				{
					_items.CacheItems.Remove(item.Id);
				}
				return item;	
			}
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (IsDisposed) return;
			IsDisposed = true;

			AbortAll();
		}
	}
}