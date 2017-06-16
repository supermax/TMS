#region Code Editor

// Maxim

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using TMS.Common.Tasks.Threading;

#endregion

namespace TMS.Common.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public static class EnumerableExtensions
	{
		///// <summary>
		///// Performs the given Action parallel for each element in the enumerable.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="that">The that.</param>
		///// <param name="action">The action to perform for each element.</param>
		///// <returns>
		///// IEnumerable of created tasks.
		///// </returns>
		//public static IEnumerable<CustomTask> ParallelForEach<T>(this IEnumerable<T> that, Action<T> action)
		//{
		//	return that.ParallelForEach(action, null);
		//}

		///// <summary>
		///// Performs the given Action parallel for each element in the enumerable.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="that">The that.</param>
		///// <param name="action">The action to perform for each element.</param>
		///// <param name="target">The TaskDistributor instance on which the operation should perform.</param>
		///// <returns>
		///// IEnumerable of created tasks.
		///// </returns>
		//public static IEnumerable<CustomTask> ParallelForEach<T>(this IEnumerable<T> that, Action<T> action,
		//	TaskDistributor target)
		//{
		//	return that.ParallelForEach(element =>
		//	{
		//		action(element);
		//		return CustomTask.Unit.Default;
		//	}, target);
		//}

		// TODO recursive ??
		///// <summary>
		///// Performs the given Func parallel for each element in the enumerable.
		///// </summary>
		///// <typeparam name="TResult">The type of the result.</typeparam>
		///// <typeparam name="T"></typeparam>
		///// <param name="that">The that.</param>
		///// <param name="action">The Func to perform for each element.</param>
		///// <returns>
		///// IEnumerable of created tasks.
		///// </returns>
		//public static IEnumerable<CustomTask<TResult>> ParallelForEach<TResult, T>(this IEnumerable<T> that,
		//	Func<T, TResult> action)
		//{
		//	return that.ParallelForEach(action);
		//}

		/// <summary>
		/// Performs the given Func parallel for each element in the enumerable.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="that">The that.</param>
		/// <param name="action">The Func to perform for each element.</param>
		/// <param name="target">The TaskDistributor instance on which the operation should perform.</param>
		/// <returns>
		/// IEnumerable of created tasks.
		/// </returns>
		public static IEnumerable<CustomTask<TResult>> ParallelForEach<TResult, T>(this IEnumerable<T> that,
			Func<T, TResult> action,
			TaskDistributor target)
		{
			var res = that.Select(tmp => CustomTask.Create(() => action(tmp)).Run(target)).CreateList();
			return res;
		}

		///// <summary>
		///// Performs the given Action sequential for each element in the enumerable.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="that">The that.</param>
		///// <param name="action">The Action to perform for each element.</param>
		///// <returns>
		///// IEnumerable of created tasks.
		///// </returns>
		//public static IEnumerable<CustomTask> SequentialForEach<T>(this IEnumerable<T> that, Action<T> action)
		//{
		//	return that.SequentialForEach(action, null);
		//}

		///// <summary>
		///// Performs the given Action sequential for each element in the enumerable.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="that">The that.</param>
		///// <param name="action">The Action to perform for each element.</param>
		///// <param name="target">The TaskDistributor instance on which the operation should perform.</param>
		///// <returns>
		///// IEnumerable of created tasks.
		///// </returns>
		//public static IEnumerable<CustomTask> SequentialForEach<T>(this IEnumerable<T> that, Action<T> action,
		//	TaskDistributor target)
		//{
		//	return that.SequentialForEach(element =>
		//	{
		//		action(element);
		//		return CustomTask.Unit.Default;
		//	}, target);
		//}

		// TODO recursive ??
		///// <summary>
		///// Performs the given Func sequential for each element in the enumerable.
		///// </summary>
		///// <typeparam name="TResult">The type of the result.</typeparam>
		///// <typeparam name="T"></typeparam>
		///// <param name="that">The that.</param>
		///// <param name="action">The Func to perform for each element.</param>
		///// <returns>
		///// IEnumerable of created tasks.
		///// </returns>
		//public static IEnumerable<CustomTask<TResult>> SequentialForEach<TResult, T>(this IEnumerable<T> that,
		//	Func<T, TResult> action)
		//{
		//	return that.SequentialForEach(action);
		//}

		/// <summary>
		/// Performs the given Func sequential for each element in the enumerable.
		/// </summary>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="that">The that.</param>
		/// <param name="action">The Func to perform for each element.</param>
		/// <param name="target">The TaskDistributor instance on which the operation should perform.</param>
		/// <returns>
		/// IEnumerable of created tasks.
		/// </returns>
		public static IEnumerable<CustomTask<TResult>> SequentialForEach<TResult, T>(this IEnumerable<T> that,
			Func<T, TResult> action, TaskDistributor target)
		{
			var result = new List<CustomTask<TResult>>();
			CustomTask lastTask = null;
			foreach (var element in that)
			{
				var tmp = element;
				var task = CustomTask.Create(() => action(tmp));
				if (lastTask == null)
					task.Run(target);
				else
					lastTask.WhenEnded(() => task.Run(target));
				lastTask = task;
				result.Add(task);
			}
			return result;
		}
	}
}