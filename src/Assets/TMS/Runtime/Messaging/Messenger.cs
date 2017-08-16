#region Usings

using TMS.Common.Logging;
using System;
using System.Collections.Generic;
using TMS.Common.Core;
using TMS.Common.Extensions;
using TMS.Common.Modularity;
using TMS.Common.Tasks.Threading;

#endregion

namespace TMS.Common.Messaging
{
	/// <summary>
	///     Messenger
	/// </summary>
	public class Messenger : Singleton<IMessenger, Messenger>, IMessenger
	{
		private readonly object _locker = new object();
		private IDictionary<Type, WeakDelegatesManager> _subscribers;
		
		/// <summary>
		///     Gets the subscribers.
		/// </summary>
		/// <value>
		///     The subscribers.
		/// </value>
		private IDictionary<Type, WeakDelegatesManager> Subscribers
		{
			get
			{
				if (_subscribers == null)
				{
					lock (_locker)
					{
						if (_subscribers == null)
						{
							_subscribers = new Dictionary<Type, WeakDelegatesManager>();

							IocManager.Default.Register<IMessenger, Messenger>(this, true); // register as singleton
						}
					}
				}
				return _subscribers;
			}
		}

		/// <summary>
		///     Subscribes the specified callback.
		/// </summary>
		/// <typeparam name="T">Payload type</typeparam>
		/// <param name="callback">The callback.</param>
		/// <param name="filter">The filter.</param>
		/// <param name="keepAlive">
		///     if set to <c>true</c> [keep alive].
		/// </param>
		public DelegateReference Subscribe<T>(Action<T> callback, Predicate<T> filter = null, bool keepAlive = false)
		{
			var manager = GetManager<T>(true);
			var delRef = manager.AddListener(callback, filter, keepAlive);
			return delRef;
		}

		/// <summary>
		///     Publishes the specified payload.
		/// </summary>
		/// <typeparam name="T">Payload type</typeparam>
		/// <param name="payload">The payload.</param>
		/// <param name="async">
		///     if set to <c>true</c> [async].
		/// </param>
		public void Publish<T>(T payload, bool async = false)
		{
			// NOTE DO NOT REMOVE <T> !!!
			// ReSharper disable once RedundantTypeArgumentsOfMethod
			PublishInternal<T>(payload, async);
		}

		/// <summary>
		///     Unsubscribes the specified callback.
		/// </summary>
		/// <typeparam name="T">Payload type</typeparam>
		/// <param name="callback">The callback.</param>
		public void Unsubscribe<T>(Action<T> callback)
		{
			var manager = GetManager<T>(false);
			if (manager == null) return;

			manager.RemoveListener(callback.CreateDelegateUniqueId());
			if (manager.GetListenersCount() > 0) return;

			_subscribers.Remove(typeof (T));
		}

		/// <summary>
		///     Unsubscribes the specified callback by delegate unique id.
		/// </summary>
		/// <param name="delegateUniqueId">The delegate unique id.</param>
		public void Unsubscribe(string delegateUniqueId)
		{
			var subscribersClone = new KeyValuePair<Type, WeakDelegatesManager>[Subscribers.Count];
			Subscribers.CopyTo(subscribersClone, 0);

			foreach (var item in subscribersClone)
			{
				var manager = item.Value;
				var listener = manager.GetListener(delegateUniqueId);
				if (listener != null)
				{
					manager.RemoveListener(delegateUniqueId);
				}
				if (manager.GetListenersCount() > 0) continue;
				Subscribers.Remove(item.Key);
			}
		}

		/// <summary>
		///     Publishes the internal.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="payload">The payload.</param>
		/// <param name="async">if set to <c>true</c> [async].</param>
		/// <returns></returns>
		internal WeakDelegatesManager PublishInternal<T>(T payload, bool async)
		{
			ArgumentValidator.AssertNotDefault(payload, "payload");
			Log(LogType.Log, "{0}.Publish(payload=\"{1}\") [START]", this, payload);

			var payloadType = typeof (T);
			var manager = GetManager(payloadType, false);
			var tType = payloadType;
			if (manager == null)
			{
				payloadType = payload.GetType();

				Log(LogType.Warning, "{0}.Publish<{1}>(payload=\"{2}\") [TYPE_MISSMATCH], retrying with type: {3}...",
					this, tType, payload, payloadType);
				manager = GetManager(payloadType, false);
			}
			if (manager == null)
			{
				Log(LogType.Warning, "{0}.Publish<{1}>(payload=\"{2}\") [TYPE_MISSMATCH], can't get manager by type: {3}. Publish aborted.",
					this, tType, payload, payloadType);
				return null;
			}

			Log(LogType.Log, "{0}.Publish(payload=\"{1}\") [MANAGER=\"{2}\"]", this, payload, manager);

			manager.Raise(async, payload);

			HandleBroadcastPayload(payload as IBroadcastPayload);

			return manager;
		}

		/// <summary>
		///     Handles the broadcast payload.
		/// </summary>
		/// <param name="payload">The payload.</param>
		private void HandleBroadcastPayload(IBroadcastPayload payload)
		{
			if (payload == null) return;
			var info = payload.Info as BroadcastPayloadInfo;
			if (info == null)
			{
				info = new BroadcastPayloadInfo
				{
					PublishStartTime = DateTime.Now
				};
				payload.Info = info;
				return;
			}

			info.PublishEndTime = DateTime.Now;

			var manager = GetManager(typeof (IBroadcastPayload), false);
			if (manager == null) return;
			manager.Raise(true, payload);
		}

		/// <summary>
		///     Gets the manager.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="createNew">if set to <c>true</c> [create new].</param>
		/// <returns></returns>
		private WeakDelegatesManager GetManager<T>(bool createNew)
		{
			var payloadType = typeof (T);
			var manager = GetManager(payloadType, createNew);
			return manager;
		}

		/// <summary>
		///     Gets the manager.
		/// </summary>
		/// <param name="payloadType">Type of the payload.</param>
		/// <param name="createNew">if set to <c>true</c> [create new].</param>
		/// <returns></returns>
		private WeakDelegatesManager GetManager(Type payloadType, bool createNew)
		{
			WeakDelegatesManager manager;
			if (!Subscribers.ContainsKey(payloadType))
			{
				if (!createNew) return null;

				manager = new WeakDelegatesManager();
				Subscribers[payloadType] = manager;
			}
			else
			{
				manager = Subscribers[payloadType];
			}
			return manager;
		}

		/// <summary>
		///     Publishes payload and unsubscribes its listener by given 'listenerId'.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="payload">The payload.</param>
		/// <param name="listenerId">The listener id.</param>
		/// <param name="async">
		///     if set to <c>true</c> [async].
		/// </param>
		public void PublishAndUnsubscribe<T>(T payload, string listenerId, bool async = false)
		{
			var manager = PublishInternal(payload, async);
			if (manager == null) return;

			manager.RemoveListener(listenerId);
			if (manager.GetListenersCount() > 0) return;

			_subscribers.Remove(typeof (T));
		}

		/// <summary>
		///     Publishes the payload and unsubscribes all its listeners.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="payload">The payload.</param>
		/// <param name="async">if set to <c>true</c> [asynchronous].</param>
		public void PublishAndUnsubscribe<T>(T payload, bool async = false)
		{
			var manager = PublishInternal(payload, async);
			if (manager == null) return;

			manager.Dispose();
			_subscribers.Remove(typeof (T));
		}

		/// <summary>
		/// Logs the specified log type.
		/// </summary>
		/// <param name="logType">Type of the log.</param>
		/// <param name="str">The string.</param>
		/// <param name="args">The arguments.</param>
		protected virtual void Log(LogType logType, string str, params object[] args)
		{
			switch (logType)
			{
				case LogType.Log:
					Loggers.Default.ConsoleLogger.Write(str, args);
					break;

				case LogType.Warning:
					Loggers.Default.ConsoleLogger.Write(LogSourceType.Warning, str, args);
					break;

				case LogType.Error:
					Loggers.Default.ConsoleLogger.Write(LogSourceType.Error, str, args);
					break;
			}
		}

		/// <summary>
		/// Log Type
		/// </summary>
		protected enum LogType
		{
			/// <summary>
			/// The log
			/// </summary>
			Log,

			/// <summary>
			/// The warning
			/// </summary>
			Warning,

			/// <summary>
			/// The error
			/// </summary>
			Error
		}

		#region OLD

		/*public string WriteAllSubscribers()
		{
			var strBuilder = new System.Text.StringBuilder();

			foreach (var subscriber in _subscribers)
			{
				//strBuilder.AppendLine(string.Format("key: {0} value: {1}", subscriber.Key, subscriber.Value));

				foreach (var subS in subscriber.Value._listeners)
				{
					if (subS.Key != null)
						strBuilder.Append(string.Format("-----subS key id: {0} - isAlive {1}", subS.Key.Id, subS.Key.IsAlive));

					if (subS.Value != null)
						strBuilder.Append(string.Format("----------subS value: {0} - isAlive {1}", subS.Value, subS.Value.IsAlive));

					strBuilder.Append("\n");
				}
			}

			return strBuilder.ToString();
		}*/

		#endregion
	}
}