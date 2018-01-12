using System.Collections;
using UnityEngine;

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// Interface Thread Helper
	/// </summary>
	public interface IThreadHelper
	{
		/// <summary>
		///     Gets the current dispatcher.
		/// </summary>
		/// <value>
		///     The current dispatcher.
		/// </value>
		CustomTaskDispatcher CurrentDispatcher { get; }

		/// <summary>
		///     Gets the current task distributor.
		/// </summary>
		/// <value>
		///     The current task distributor.
		/// </value>
		TaskDistributor CurrentTaskDistributor { get; }

		/// <summary>
		/// Determines whether called method is on main thread
		/// </summary>
		/// <returns></returns>
		bool IsMainThread();

		/// <summary>
		/// Starts the coroutine.
		/// </summary>
		/// <param name="routine">The routine.</param>
		/// <returns></returns>
		Coroutine StartCoroutine(IEnumerator routine);

		/// <summary>
		/// Starts the coroutine.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		Coroutine StartCoroutine(string methodName, object value);

		/// <summary>
		/// Starts the coroutine.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <returns></returns>
		Coroutine StartCoroutine(string methodName);

		/// <summary>
		/// Stops the coroutine.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		void StopCoroutine(string methodName);

		/// <summary>
		/// Stops the coroutine.
		/// </summary>
		/// <param name="routine">The routine.</param>
		void StopCoroutine(IEnumerator routine);

		/// <summary>
		/// Stops the coroutine.
		/// </summary>
		/// <param name="routine">The routine.</param>
		void StopCoroutine(Coroutine routine);

		/// <summary>
		/// Stops all coroutines.
		/// </summary>
		void StopAllCoroutines();
	}
}