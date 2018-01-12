#region Code Editor

// Maxim

#endregion

using TMS.Common.Tasks.Threading;

namespace TMS.Common.Extensions
{
	/// <summary>
	/// </summary>
	public static class ObjectExtensions
	{
		/// <summary>
		///     Starts the given Method as async Task on the given TaskDistributor.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="args">Optional arguments passed to the method.</param>
		/// <returns>
		///     The task.
		/// </returns>
		public static CustomTask RunAsync(this object that, string methodName, params object[] args)
		{
			return that.RunAsync<object>(methodName, null, args);
		}

		/// <summary>
		///     Starts the given Method as async Task on the given TaskDistributor.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="target">The TaskDistributor instance on which the operation should perform.</param>
		/// <param name="args">Optional arguments passed to the method.</param>
		/// <returns>
		///     The task.
		/// </returns>
		public static CustomTask RunAsync(this object that, string methodName, TaskDistributor target, params object[] args)
		{
			return that.RunAsync<object>(methodName, target, args);
		}

		/// <summary>
		///     Starts the given Method as async Task on the given TaskDistributor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="that">The that.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="args">Optional arguments passed to the method.</param>
		/// <returns>
		///     The task.
		/// </returns>
		public static CustomTask<T> RunAsync<T>(this object that, string methodName, params object[] args)
		{
			return that.RunAsync<T>(methodName, null, args);
		}

		/// <summary>
		///     Starts the given Method as async Task on the given TaskDistributor.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="that">The that.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="target">The TaskDistributor instance on which the operation should perform.</param>
		/// <param name="args">Optional arguments passed to the method.</param>
		/// <returns>
		///     The task.
		/// </returns>
		public static CustomTask<T> RunAsync<T>(this object that, string methodName, TaskDistributor target,
			params object[] args)
		{
			return CustomTask.Create<T>(that, methodName, args).Run(target);
		}
	}
}