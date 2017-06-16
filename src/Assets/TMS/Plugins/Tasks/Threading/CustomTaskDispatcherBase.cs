using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMS.Common.Extensions;

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	public abstract class CustomTaskDispatcherBase : IDisposable
	{
		protected ManualResetEvent DataEvent = new ManualResetEvent(false);

		protected Queue<CustomTask> DelayedTaskList = new Queue<CustomTask>();

		protected int LockCount = 0;

		protected Queue<CustomTask> TaskList = new Queue<CustomTask>();

		protected object TaskListSyncRoot = new object();

        /// <summary>
		///     Gets or sets a value indicating whether [allow access limitation checks].
		/// </summary>
		/// <value>
		///     <c>true</c> if [allow access limitation checks]; otherwise, <c>false</c>.
		/// </value>
		public virtual bool AllowAccessLimitationChecks { get; set; }

		/// <summary>
		///     Set the task reordering system
		/// </summary>
		public virtual TaskSortingSystem TaskSortingSystem { get; set; }

		/// <summary>
		///     Gets a value indicating whether [is working].
		/// </summary>
		/// <value>
		///     <c>true</c> if [is working]; otherwise, <c>false</c>.
		/// </value>
		public bool IsWorking
		{
			get { return DataEvent.InterWaitOne(0); }
		}

		/// <summary>
		///     Returns the currently existing task count. Early aborted tasks will count too.
		/// </summary>
		public virtual int TaskCount
		{
			get
			{
				lock (TaskListSyncRoot)
				{
					return TaskList.Count;
				}
			}
		}

		/// <summary>
		///     Locks this instance.
		/// </summary>
		public void Lock()
		{
			lock (TaskListSyncRoot)
			{
				LockCount++;
			}
		}

		/// <summary>
		///     Unlocks this instance.
		/// </summary>
		public void Unlock()
		{
			lock (TaskListSyncRoot)
			{
				LockCount--;
				if (LockCount != 0 || DelayedTaskList.Count <= 0) return;

				while (DelayedTaskList.Count > 0)
				{
					TaskList.Enqueue(DelayedTaskList.Dequeue());
				}

				if (TaskSortingSystem == TaskSortingSystem.ReorderWhenAdded ||
				    TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
				{
					ReorderTasks();
				}

				TasksAdded();
			}
		}

		/// <summary>
		///     Creates a new Task based upon the given action.
		/// </summary>
		/// <typeparam name="T">The return value of the task.</typeparam>
		/// <param name="function">The function to process at the dispatchers thread.</param>
		/// <returns>The new task.</returns>
		public CustomTask<T> Dispatch<T>(Func<T> function)
		{
			CheckAccessLimitation();

			var task = new CustomTask<T>(function);
			AddTask(task);
			return task;
		}

		/// <summary>
		///     Creates a new Task based upon the given action.
		/// </summary>
		/// <param name="action">The action to process at the dispatchers thread.</param>
		/// <returns>The new task.</returns>
		public CustomTask Dispatch(Action action)
		{
			CheckAccessLimitation();

			var task = CustomTask.Create(action);
			AddTask(task);
			return task;
		}

		/// <summary>
		///     Dispatches a given Task.
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns>
		///     The new task.
		/// </returns>
		public CustomTask Dispatch(CustomTask task)
		{
			CheckAccessLimitation();

			AddTask(task);
			return task;
		}

		/// <summary>
		///     Adds the task.
		/// </summary>
		/// <param name="task">The task.</param>
		internal virtual void AddTask(CustomTask task)
		{
			lock (TaskListSyncRoot)
			{
				if (LockCount > 0)
				{
					DelayedTaskList.Enqueue(task);
					return;
				}

				TaskList.Enqueue(task);

				if (TaskSortingSystem == TaskSortingSystem.ReorderWhenAdded ||
				    TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
					ReorderTasks();
			}
			TasksAdded();
		}

		/// <summary>
		///     Adds the tasks.
		/// </summary>
		/// <param name="tasks">The tasks.</param>
		internal void AddTasks(IEnumerable<CustomTask> tasks)
		{
			lock (TaskListSyncRoot)
			{
				if (LockCount > 0)
				{
					foreach (var task in tasks)
						DelayedTaskList.Enqueue(task);
					return;
				}

				foreach (var task in tasks)
					TaskList.Enqueue(task);

				if (TaskSortingSystem == TaskSortingSystem.ReorderWhenAdded ||
				    TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
					ReorderTasks();
			}
			TasksAdded();
		}

		internal virtual void TasksAdded()
		{
			DataEvent.Set();
		}

		/// <summary>
		///     Reorders the tasks.
		/// </summary>
		protected void ReorderTasks()
		{
			TaskList = new Queue<CustomTask>(TaskList.OrderBy(t => t.Priority));
		}

		/// <summary>
		///     Splits the tasks.
		/// </summary>
		/// <param name="divisor">The divisor.</param>
		/// <returns></returns>
		internal IEnumerable<CustomTask> SplitTasks(int divisor)
		{
			if (divisor == 0)
				divisor = 2;
			var count = TaskCount/divisor;
			return IsolateTasks(count);
		}

		internal IEnumerable<CustomTask> IsolateTasks(int count)
		{
			var newTasks = new Queue<CustomTask>();

			if (count == 0)
				count = TaskList.Count;

			lock (TaskListSyncRoot)
			{
				for (var i = 0; i < count && i < TaskList.Count; ++i)
					newTasks.Enqueue(TaskList.Dequeue());

				//if (TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
				//    taskList = ReorderTasks(taskList);

				if (TaskCount == 0)
					DataEvent.Reset();
			}

			return newTasks;
		}

		/// <summary>
		///     Checks the access limitation.
		/// </summary>
		protected abstract void CheckAccessLimitation();

        #region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			while (true)
			{
				CustomTask currentTask;
				lock (TaskListSyncRoot)
				{
					if (TaskList.Count != 0)
						currentTask = TaskList.Dequeue();
					else
						break;
				}
				currentTask.Dispose();
			}

			DataEvent.Close();
			DataEvent = null;
		}

		#endregion
	}
}