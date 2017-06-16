#region Code Editor

// Maxim

#endregion

#region

using System;
using TMS.Common.Tasks.Threading;

#endregion

namespace TMS.Common.Extensions
{
	/// <summary>
	/// </summary>
	public static class ActionExtensions
	{
		/// <summary>
		///     Starts the Action as async Task.
		/// </summary>
		/// <returns>The task.</returns>
		public static CustomTask RunAsync(this Action that)
		{
			return that.RunAsync(ThreadHelper.TaskDistributor);
		}

		/// <summary>
		///     Starts the Action as async Task on the given TaskDistributor.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="target">The TaskDistributor instance on which the operation should perform.</param>
		/// <returns>
		///     The task.
		/// </returns>
		public static CustomTask RunAsync(this Action that, TaskDistributor target)
		{
			return target.Dispatch(that);
		}

		/// <summary>
		///     Converts the Action into an inactive Task.
		/// </summary>
		/// <returns>The task.</returns>
		public static CustomTask AsTask(this Action that)
		{
			return CustomTask.Create(that);
		}

		/// <summary>
		///     Starts the Func as async Task.
		/// </summary>
		/// <returns>The task.</returns>
		public static CustomTask<T> RunAsync<T>(this Func<T> that)
		{
			return that.RunAsync(ThreadHelper.TaskDistributor);
		}

		/// <summary>
		///     Starts the Func as async Task on the given TaskDistributor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="that">The that.</param>
		/// <param name="target">The TaskDistributor instance on which the operation should perform.</param>
		/// <returns>
		///     The task.
		/// </returns>
		public static CustomTask<T> RunAsync<T>(this Func<T> that, TaskDistributor target)
		{
			return target.Dispatch(that);
		}

		/// <summary>
		///     Converts the Func into an inactive Task.
		/// </summary>
		/// <returns>The task.</returns>
		public static CustomTask<T> AsTask<T>(this Func<T> that)
		{
			return new CustomTask<T>(that);
		}
	}
}