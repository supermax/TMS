using TMS.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TMS.Common.Extensions;

namespace TMS.Common.Core
{
	/// <summary>
	///     Weak Delegates Manager
	/// </summary>
	internal class WeakDelegatesManager : IDisposable
	{
		private readonly Dictionary<DelegateReference, DelegateReference> _listeners =
			new Dictionary<DelegateReference, DelegateReference>();

		/// <summary>
		///     Occurs when [delegate invoking event].
		/// </summary>
		public virtual event EventHandler<WeakDelegatesManagerEventArgs> DelegateInvokingEvent;

		/// <summary>
		/// Gets the listeners count.
		/// </summary>
		/// <returns></returns>
		internal int GetListenersCount()
		{
			return _listeners.Count;
		}

		/// <summary>
		///     Called when [delegate invoking event].
		/// </summary>
		/// <param name="delRef">The delete preference.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		protected virtual bool OnDelegateInvokingEvent(DelegateReference delRef, params object[] args)
		{
			if (DelegateInvokingEvent == null) return false;
			var arg = new WeakDelegatesManagerEventArgs(delRef);
			DelegateInvokingEvent(this, arg);
			return arg.IsHandled;
		}

		/// <summary>
		///     Occurs when [delegate invoked event].
		/// </summary>
		public event EventHandler<WeakDelegatesManagerEventArgs> DelegateInvokedEvent;

		/// <summary>
		///     Called when [delegate invoked event].
		/// </summary>
		/// <param name="delRef">The delete preference.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		protected virtual bool OnDelegateInvokedEvent(DelegateReference delRef, params object[] args)
		{
			if (DelegateInvokedEvent == null) return false;
			var arg = new WeakDelegatesManagerEventArgs(delRef);
			DelegateInvokedEvent(this, arg);
			return arg.IsHandled;
		}

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_listeners.ForEach(item =>
			{
				if (item.Key != null)
				{
					item.Key.Dispose();
				}
				if (item.Value != null)
				{
					item.Value.Dispose();
				}
			});
			_listeners.Clear();
		}

		/// <summary>
		///     Adds the listener.
		/// </summary>
		/// <param name="listener">The listener.</param>
		/// <param name="filter">The filter.</param>
		/// <param name="keepAlive">if set to <c>true</c> [keep alive].</param>
		public DelegateReference AddListener(Delegate listener, Delegate filter = null, bool keepAlive = false)
		{
			var id =  listener.CreateDelegateUniqueId();
			var delRefs = GetListeners(id);
			if (!delRefs.IsNullOrEmpty())
			{
				var fid = filter != null ? filter.CreateDelegateUniqueId() : listener.CreateDelegateUniqueId();
				foreach (var dr in delRefs)
				{
					if(dr == null) continue;
					if (dr.Id == id || _listeners[dr].Id == fid) return dr;
				}
			}

			var delRef = new DelegateReference(listener, keepAlive);
			var filRef = filter == null ? null : new DelegateReference(filter, keepAlive);

			_listeners.Add(delRef, filRef);

			return delRef;
		}

		/// <summary>
		///     Removes the listener.
		/// </summary>
		/// <param name="listener">The listener.</param>
		/// <returns></returns>
		public bool RemoveListener(Delegate listener)
		{
			var listenerId = listener.CreateDelegateUniqueId();
			return RemoveListener(listenerId);
		}

		/// <summary>
		///     Removes the listener.
		/// </summary>
		/// <param name="listenerId">The listener id.</param>
		/// <returns></returns>
		public bool RemoveListener(string listenerId)
		{
			var res = GetListeners(listenerId);
			foreach (var key in res)
			{
				_listeners.Remove(key);
			}
			return res.Length > 0;
		}

		/// <summary>
		///     Gets the listeners.
		/// </summary>
		/// <param name="listenerId">The listener id.</param>
		/// <returns></returns>
		private DelegateReference[] GetListeners(string listenerId)
		{
			var list = new List<DelegateReference>();
			foreach (var pair in _listeners)
			{
				if (pair.Key.Id != listenerId) continue;
				list.Add(pair.Key);
			}
			var res = list.ToArray();
			return res;
		}

		/// <summary>
		///     Gets the listener.
		/// </summary>
		/// <param name="listenerId">The listener id.</param>
		/// <returns></returns>
		public DelegateReference GetListener(string listenerId)
		{
			DelegateReference res = null;
			foreach (var pair in _listeners)
			{
				if (pair.Key.Id != listenerId) continue;
				res = pair.Key;
				break;
			}
			return res;
		}

		private DelegateReference[] GetDeadListeners()
		{
			var list = new List<DelegateReference>();
			foreach (var pair in _listeners)
			{
				if (!pair.Key.IsAlive)
				{
					list.Add(pair.Key);
				}
			}
			var res = list.ToArray();
			return res;
		}

		/// <summary>
		///     Cleanups this instance.
		/// </summary>
		private void Cleanup()
		{
			if (_listeners.IsNullOrEmpty()) return;

			var res = GetDeadListeners();
			if (res.IsNullOrEmpty()) return;

			// NOTE: Workaround for iOS
			var keys = new DelegateReference[res.Length];
			for (var i = 0; i < keys.Length; i++)
			{
				keys[i] = res[i];
			}

			foreach (var key in keys)
			{
				Loggers.Default.ConsoleLogger.Write("Listener_Cleanup [{0}=\"{1}\"]", key, key.Id);
				_listeners.Remove(key);
			}
		}

		/// <summary>
		///     Raises the specified async.
		/// </summary>
		/// <param name="async">if set to <c>true</c> [async].</param>
		/// <param name="args">The args.</param>
		public void Raise(bool async = false, params object[] args)
		{
			Cleanup();
			if (_listeners.IsNullOrEmpty()) return;

			// NOTE: Workaround for iOS
			var listenersClone = _listeners.CreateArray();
			
			// invoke callbacks
			foreach (var handler in listenersClone)
			{
				var listener = handler.Key;
				var isHandled = OnDelegateInvokingEvent(listener);
				if (isHandled) break;

				var filter = handler.Value;
				if (filter != null)
				{
					var canInvoke = (bool) filter.Invoke(args);
					//Debug.WriteLine(string.Format("Listener_Filter_Invoke [canInvoke=\"{0}\", args=\"{1}\"]", canInvoke, args));
					if (!canInvoke) continue;
				}

				if (async)
				{
					//Debug.WriteLine(string.Format("Listener_BeginInvoke [{0}=\"{1}\"]", listener, listener.Id));
					listener.BeginInvoke(args); // TODO test in unity if opens new thread
				}
				else
				{
					//Debug.WriteLine(string.Format("Listener_Invoke [{0}=\"{1}\"]", listener, listener.Id));
					listener.Invoke(args);
				}

				isHandled = OnDelegateInvokedEvent(listener);
				if (isHandled) break;
			}
		}
	}

	internal class WeakDelegatesManagerEventArgs : EventArgs
	{
		public WeakDelegatesManagerEventArgs(DelegateReference delRef, params object[] args)
		{
			DelegateRef = delRef;
			Args = args;
		}

		public DelegateReference DelegateRef { get; private set; }

		public IEnumerable<object> Args { get; private set; }

		public bool IsHandled { get; set; }
	}
}