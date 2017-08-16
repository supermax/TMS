#region Usings

using System;
using TMS.Common.Core;

#endregion

namespace TMS.Common.Messaging
{
	/// <summary>
	///     Interface for Messenger
	/// </summary>
	public interface IMessenger
	{
		/// <summary>
		///     Subscribes the specified callback.
		/// </summary>
		/// <typeparam name="T">Payload type</typeparam>
		/// <param name="callback">The callback.</param>
		/// <param name="filter">The filter.</param>
		/// <param name="keepAlive">
		///     if set to <c>true</c> [keep alive].
		/// </param>
		DelegateReference Subscribe<T>(Action<T> callback, Predicate<T> filter = null, bool keepAlive = false);

		/// <summary>
		///     Unsubscribes the specified callback.
		/// </summary>
		/// <typeparam name="T">Payload type</typeparam>
		/// <param name="callback">The callback.</param>
		void Unsubscribe<T>(Action<T> callback);

		/// <summary>
		///     Unsubscribes the specified callback by delegate unique id.
		/// </summary>
		/// <param name="delegateUniqueId">The delegate unique id.</param>
		void Unsubscribe(string delegateUniqueId);

		/// <summary>
		///     Publishes the specified payload.
		/// </summary>
		/// <typeparam name="T">Payload type</typeparam>
		/// <param name="payload">The payload.</param>
		/// <param name="async">
		///     if set to <c>true</c> [asynchronous].
		/// </param>
		void Publish<T>(T payload, bool async = false);

		/// <summary>
		///     Publishes the payload and unsubscribes its listener by given 'listenerId'.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="payload">The payload.</param>
		/// <param name="listenerId">The listener identifier.</param>
		/// <param name="async">if set to <c>true</c> [asynchronous].</param>
		void PublishAndUnsubscribe<T>(T payload, string listenerId, bool async = false);

		/// <summary>
		///     Publishes the payload and unsubscribes all its listeners.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="payload">The payload.</param>
		/// <param name="async">if set to <c>true</c> [asynchronous].</param>
		void PublishAndUnsubscribe<T>(T payload, bool async = false);
	}
}