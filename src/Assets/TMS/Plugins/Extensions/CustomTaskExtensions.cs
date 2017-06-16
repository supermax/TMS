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
	public static class CustomTaskExtensions
	{
		/// <summary>
		///     Sets the name of the task
		/// </summary>
		public static CustomTask WithName(this CustomTask task, string name)
		{
			task.Name = name;
			return task;
		}

		/// <summary>
		///     Sets the name of the task
		/// </summary>
		public static CustomTask<T> WithName<T>(this CustomTask<T> task, string name)
		{
			task.Name = name;
			return task;
		}

		/// <summary>
		///     Waits for the completion of all tasks in the Enumerable.
		/// </summary>
		public static void WaitAll(this IEnumerable<CustomTask> tasks)
		{
			foreach (var task in tasks)
				task.Wait();
		}

		/// <summary>
		/// Starts the given Task when the tasks ended successfully.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="followingTask">The task to start.</param>
		/// <param name="target">The DispatcherBase to start the following task on.</param>
		/// <returns>
		/// The tasks.
		/// </returns>
		public static IEnumerable<CustomTask> Then(this IEnumerable<CustomTask> that, CustomTask followingTask,
			CustomTaskDispatcherBase target)
		{
			var remaining = that.Count();
			var syncRoot = new object();

			foreach (var task in that)
			{
				task.WhenFailed(() =>
				{
					if (followingTask.ShouldAbort)
						return;
					followingTask.Abort();
				});
				task.WhenSucceeded(() =>
				{
					if (followingTask.ShouldAbort)
						return;

					lock (syncRoot)
					{
						remaining--;
						if (remaining != 0) return;

						if (target != null)
							followingTask.Run(target);
						else if (CustomThread.CurrentThread is TaskWorker)
							followingTask.Run(((TaskWorker) CustomThread.CurrentThread).TaskDistributor);
						else
							followingTask.Run();
					}
				});
			}
			return that;
		}

		/// <summary>
		/// Starts the given Action when all Tasks ended successfully.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="action">The action to start.</param>
		/// <param name="target">The DispatcherBase to start the following action on.</param>
		/// <returns>
		/// The tasks.
		/// </returns>
		public static IEnumerable<CustomTask> WhenSucceeded(this IEnumerable<CustomTask> that, Action action,
			CustomTaskDispatcherBase target)
		{
			var remaining = that.Count();
			var syncRoot = new object();

			foreach (var task in that)
			{
				task.WhenSucceeded(() =>
				{
					lock (syncRoot)
					{
						remaining--;
						if (remaining != 0) return;

						if (target == null)
							action();
						else
							target.Dispatch(action);
					}
				});
			}
			return that;
		}

		/// <summary>
		/// Starts the given Action when one task has not successfully ended
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="action">The action to start.</param>
		/// <param name="target">The DispatcherBase to start the following action on.</param>
		/// <returns>
		/// The tasks.
		/// </returns>
		public static IEnumerable<CustomTask> WhenFailed(this IEnumerable<CustomTask> that, Action action,
			CustomTaskDispatcherBase target)
		{
			var hasFailed = false;
			var syncRoot = new object();
			foreach (var task in that)
			{
				task.WhenFailed(() =>
				{
					lock (syncRoot)
					{
						if (hasFailed)
							return;
						hasFailed = true;

						if (target == null)
							action();
						else
							target.Dispatch(action);
					}
				});
			}
			return that;
		}

		/// <summary>
		/// Invokes the given action with the set result of the task when the task succeeded.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask OnResult(this CustomTask task, Action<object> action)
		{
			return task.OnResult(action, null);
		}

		/// <summary>
		/// Invokes the given action with the set result of the task when the task succeeded.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="target">The DispatcherBase to perform the action on.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask OnResult(this CustomTask task, Action<object> action, CustomTaskDispatcherBase target)
		{
			return task.WhenSucceeded(t => action(t.RawResult), target);
		}

		/// <summary>
		/// Invokes the given action with the set result of the task when the task succeeded.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask OnResult<T>(this CustomTask task, Action<T> action)
		{
			return task.OnResult(action, null);
		}

		/// <summary>
		/// Invokes the given action with the set result of the task when the task succeeded.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="target">The DispatcherBase to perform the action on.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask OnResult<T>(this CustomTask task, Action<T> action, CustomTaskDispatcherBase target)
		{
			return task.WhenSucceeded(t => action((T) t.RawResult), target);
		}

		/// <summary>
		/// Invokes the given action with the set result of the task when the task succeeded.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> OnResult<T>(this CustomTask<T> task, Action<T> action)
		{
			return task.OnResult(action, null);
		}

		/// <summary>
		/// Invokes the given action with the set result of the task when the task succeeded.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="actionTarget">The action target.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> OnResult<T>(this CustomTask<T> task, Action<T> action, CustomTaskDispatcherBase actionTarget)
		{
			return task.WhenSucceeded(t => action(t.Result), actionTarget);
		}

		/// <summary>
		/// Starts the given Task when this Task ended successfully.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="followingTask">The task to start.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask Then(this CustomTask that, CustomTask followingTask)
		{
			TaskDistributor target = null;
			if (CustomThread.CurrentThread is TaskWorker)
			{
				target = ((TaskWorker) CustomThread.CurrentThread).TaskDistributor;
			}
			return that.Then(followingTask, target);
		}

		/// <summary>
		/// Starts the given Task when this Task ended successfully.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="followingTask">The task to start.</param>
		/// <param name="target">The DispatcherBase to start the following task on.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask Then(this CustomTask that, CustomTask followingTask, CustomTaskDispatcherBase target)
		{
			that.WhenFailed(followingTask.Abort);
			that.WhenSucceeded(() =>
			{
				if (target != null)
					followingTask.Run(target);
				else if (CustomThread.CurrentThread is TaskWorker)
					followingTask.Run(((TaskWorker) CustomThread.CurrentThread).TaskDistributor);
				else
					followingTask.Run();
			});
			return that;
		}

		/// <summary>
		/// Starts this Task when the other Task ended successfully.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="taskToWaitFor">The task to wait for.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask Await(this CustomTask that, CustomTask taskToWaitFor)
		{
			taskToWaitFor.Then(that);
			return that;
		}

		/// <summary>
		/// Starts this Task when the other Task ended successfully.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="taskToWaitFor">The task to wait for.</param>
		/// <param name="target">The DispatcherBase to start this task on.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask Await(this CustomTask that, CustomTask taskToWaitFor, CustomTaskDispatcherBase target)
		{
			taskToWaitFor.Then(that, target);
			return that;
		}

		/// <summary>
		///     Converts this Task.
		/// </summary>
		/// <param name="that"></param>
		/// <returns>The converted task.</returns>
		public static CustomTask<T> As<T>(this CustomTask that)
		{
			return (CustomTask<T>) that;
		}

		/// <summary>
		/// Starts the given Action when any Task in the Enumerable has ended.
		/// </summary>
		/// <param name="tasks">The tasks.</param>
		/// <param name="action">The action to start.</param>
		/// <returns>
		/// This Enumerable of Tasks.
		/// </returns>
		public static IEnumerable<CustomTask> ContinueWhenAnyEnded(this IEnumerable<CustomTask> tasks, Action action)
		{
			return tasks.ContinueWhenAnyEnded(t => action());
		}

		/// <summary>
		/// Starts the given Action when any Task in the Enumerable has ended.
		/// </summary>
		/// <param name="tasks">The tasks.</param>
		/// <param name="action">The action to start.</param>
		/// <returns>
		/// This Enumerable of Tasks.
		/// </returns>
		public static IEnumerable<CustomTask> ContinueWhenAnyEnded(this IEnumerable<CustomTask> tasks,
			Action<CustomTask> action)
		{
			var syncRoot = new object();
			var done = false;
			foreach (var task in tasks)
			{
				task.WhenEnded(t =>
				{
					lock (syncRoot)
					{
						if (done)
							return;

						done = true;
						action(t);
					}
				});
			}

			return tasks;
		}

		/// <summary>
		/// Starts the given Action when all Tasks in the Enumerable have ended.
		/// </summary>
		/// <param name="tasks">The tasks.</param>
		/// <param name="action">The action to start.</param>
		/// <returns>
		/// This Enumerable of Tasks.
		/// </returns>
		public static IEnumerable<CustomTask> ContinueWhenAllEnded(this IEnumerable<CustomTask> tasks, Action action)
		{
			return tasks.ContinueWhenAllEnded(t => action());
		}

		/// <summary>
		/// Starts the given Action when all Tasks in the Enumerable have ended.
		/// </summary>
		/// <param name="tasks">The tasks.</param>
		/// <param name="action">The action to start.</param>
		/// <returns>
		/// This Enumerable of Tasks.
		/// </returns>
		public static IEnumerable<CustomTask> ContinueWhenAllEnded(this IEnumerable<CustomTask> tasks,
			Action<IEnumerable<CustomTask>> action)
		{
			var count = tasks.Count();

			if (count == 0)
				action(new CustomTask[0]);

			var finishedTasks = new List<CustomTask>();
			var syncRoot = new object();

			foreach (var task in tasks)
			{
				task.WhenEnded(t =>
				{
					lock (syncRoot)
					{
						finishedTasks.Add(task);
						if (finishedTasks.Count == count)
							action(finishedTasks);
					}
				});
			}

			return tasks;
		}

		#region Succeeded

		/// <summary>
		/// The given Action will be performed when the task succeeds.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> WhenSucceeded<T>(this CustomTask<T> task, Action action)
		{
			return task.WhenSucceeded(t => action(), null);
		}

		/// <summary>
		/// The given Action will be performed when the task succeeds.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> WhenSucceeded<T>(this CustomTask<T> task, Action<CustomTask<T>> action)
		{
			return task.WhenSucceeded(action, null);
		}

		/// <summary>
		/// The given Action will be performed when the task succeeds.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="target">The DispatcherBase to perform the action on.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> WhenSucceeded<T>(this CustomTask<T> task, Action<CustomTask<T>> action,
			CustomTaskDispatcherBase target)
		{
			Action<CustomTask<T>> perform = t =>
			{
				if (target == null)
					action(t);
				else
					target.Dispatch(() => { if (t.IsSucceeded) action(t); });
			};

			return task.WhenEnded(t => { if (t.IsSucceeded) perform(t); }, null);
		}

		/// <summary>
		/// The given Action will be performed when the task succeeds.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask WhenSucceeded(this CustomTask task, Action action)
		{
			return task.WhenEnded(t => { if (t.IsSucceeded) action(); });
		}

		/// <summary>
		/// The given Action will be performed when the task succeeds.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask WhenSucceeded(this CustomTask task, Action<CustomTask> action)
		{
			return task.WhenEnded(t => { if (t.IsSucceeded) action(t); });
		}

		/// <summary>
		/// The given Action will be performed when the task succeeds.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="actionTarget">The action target.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask WhenSucceeded(this CustomTask task, Action<CustomTask> action,
			CustomTaskDispatcherBase actionTarget)
		{
			Action<CustomTask> perform = t =>
			{
				if (actionTarget == null)
					action(t);
				else
					actionTarget.Dispatch(() => { if (t.IsSucceeded) action(t); });
			};

			return task.WhenEnded(t => { if (t.IsSucceeded) perform(t); }, null);
		}

		#endregion

		#region Failed

		/// <summary>
		/// The given Action will be performed when the task fails.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> WhenFailed<T>(this CustomTask<T> task, Action action)
		{
			return task.WhenFailed(t => action(), null);
		}

		/// <summary>
		/// The given Action will be performed when the task fails.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> WhenFailed<T>(this CustomTask<T> task, Action<CustomTask<T>> action)
		{
			return task.WhenFailed(action, null);
		}

		/// <summary>
		/// The given Action will be performed when the task fails.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="target">The DispatcherBase to perform the action on.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> WhenFailed<T>(this CustomTask<T> task, Action<CustomTask<T>> action, CustomTaskDispatcherBase target)
		{
			return task.WhenEnded(t => { if (t.IsFailed) action(t); }, target);
		}

		/// <summary>
		/// The given Action will be performed when the task fails.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask WhenFailed(this CustomTask task, Action action)
		{
			return task.WhenEnded(t => { if (t.IsFailed) action(); });
		}

		/// <summary>
		/// The given Action will be performed when the task fails.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask WhenFailed(this CustomTask task, Action<CustomTask> action)
		{
			return task.WhenEnded(t => { if (t.IsFailed) action(t); });
		}

		/// <summary>
		/// The given Action will be performed when the task fails.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="target">The DispatcherBase to perform the action on.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask WhenFailed(this CustomTask task, Action<CustomTask> action, CustomTaskDispatcherBase target)
		{
			return task.WhenEnded(t => { if (t.IsFailed) action(t); }, target);
		}

		#endregion

		#region Ended

		/// <summary>
		/// The given Action will be performed when the task ends.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> WhenEnded<T>(this CustomTask<T> task, Action action)
		{
			return task.WhenEnded(t => action(), null);
		}

		/// <summary>
		/// The given Action will be performed when the task ends.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> WhenEnded<T>(this CustomTask<T> task, Action<CustomTask<T>> action)
		{
			return task.WhenEnded(action, null);
		}

		/// <summary>
		/// The given Action will be performed when the task ends.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="target">The DispatcherBase to perform the action on.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask<T> WhenEnded<T>(this CustomTask<T> task, Action<CustomTask<T>> action, CustomTaskDispatcherBase target)
		{
			task.TaskEnded += t =>
			{
				if (target == null)
					action(task);
				else
					target.Dispatch(() => action(task));
			};

			return task;
		}

		/// <summary>
		/// The given Action will be performed when the task ends.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask WhenEnded(this CustomTask task, Action action)
		{
			return task.WhenEnded(t => action());
		}

		/// <summary>
		/// The given Action will be performed when the task ends.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask WhenEnded(this CustomTask task, Action<CustomTask> action)
		{
			return task.WhenEnded(action, null);
		}

		/// <summary>
		/// The given Action will be performed when the task ends.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <param name="action">The action to perform.</param>
		/// <param name="target">The DispatcherBase to perform the action on.</param>
		/// <returns>
		/// This task.
		/// </returns>
		public static CustomTask WhenEnded(this CustomTask task, Action<CustomTask> action, CustomTaskDispatcherBase target)
		{
			task.TaskEnded += (t) =>
			{
				if (target == null)
					action(task);
				else
					target.Dispatch(() => action(task));
			};

			return task;
		}

		#endregion
	}
}