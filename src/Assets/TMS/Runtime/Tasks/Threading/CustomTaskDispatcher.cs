#region Code Editor

// Maxim

#endregion

using TMS.Common.Extensions;

#region

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// 
	/// </summary>
	public class CustomTaskDispatcher : CustomTaskDispatcherBase
	{
		[ThreadStatic] private static CustomTask _currentTask;

		[ThreadStatic] internal static CustomTaskDispatcher CurrentDispatcher;

		protected static CustomTaskDispatcher MainDispatcher;

		/// <summary>
		///     Creates a Dispatcher, if a Dispatcher has been created in the current thread an exception will be thrown.
		/// </summary>
		public CustomTaskDispatcher()
			: this(true)
		{
		}

		/// <summary>
		///     Creates a Dispatcher, if a Dispatcher has been created when setThreadDefaults is set to true in the current thread
		///     an exception will be thrown.
		/// </summary>
		/// <param name="setThreadDefaults">If set to true the new dispatcher will be set as threads default dispatcher.</param>
		public CustomTaskDispatcher(bool setThreadDefaults)
		{
			if (!setThreadDefaults)
				return;

			if (CurrentDispatcher != null)
				throw new InvalidOperationException("Only one Dispatcher instance allowed per thread.");

			CurrentDispatcher = this;

			if (MainDispatcher == null)
				MainDispatcher = this;
		}

		/// <summary>
		///     Returns the task which is currently being processed. Use this only inside a task operation.
		/// </summary>
		public static CustomTask CurrentTask
		{
			get
			{
				if (_currentTask == null)
					throw new InvalidOperationException("No task is currently running.");

				return _currentTask;
			}
		}

		/// <summary>
		///     Returns the Dispatcher instance of the current thread. When no instance has been created an exception will be
		///     thrown.
		/// </summary>
		public static CustomTaskDispatcher Current
		{
			get
			{
				if (CurrentDispatcher == null)
					throw new InvalidOperationException(
						"No Dispatcher found for the current thread, please create a new Dispatcher instance before calling this property.");
				return CurrentDispatcher;
			}
			set
			{
				if (CurrentDispatcher != null)
					CurrentDispatcher.Dispose();
				CurrentDispatcher = value;
			}
		}

		/// <summary>
		///     Returns the Dispatcher instance of the current thread.
		/// </summary>
		public static CustomTaskDispatcher CurrentNoThrow
		{
			get { return CurrentDispatcher; }
		}

		/// <summary>
		///     Returns the first created Dispatcher instance, in most cases this will be the Dispatcher for the main thread. When
		///     no instance has been created an exception will be thrown.
		/// </summary>
		public static CustomTaskDispatcher Main
		{
			get
			{
				if (MainDispatcher == null)
					throw new InvalidOperationException(
						"No Dispatcher found for the main thread, please create a new Dispatcher instance before calling this property.");

				return MainDispatcher;
			}
		}

		/// <summary>
		///     Returns the first created Dispatcher instance.
		/// </summary>
		public static CustomTaskDispatcher MainNoThrow
		{
			get { return MainDispatcher; }
		}

		/// <summary>
		///     Creates a new function based upon an other function which will handle exceptions. Use this to wrap safe functions
		///     for tasks.
		/// </summary>
		/// <typeparam name="T">The return type of the function.</typeparam>
		/// <param name="function">The original function.</param>
		/// <returns>The safe function.</returns>
		public static Func<T> CreateSafeFunction<T>(Func<T> function)
		{
			return () =>
			{
				try
				{
					return function();
				}
				catch
				{
					CurrentTask.Abort();
					return default(T);
				}
			};
		}

		/// <summary>
		/// Creates a new action based upon an other action which will handle exceptions. Use this to wrap safe action for
		/// tasks.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns>
		/// The safe action.
		/// </returns>
		public static Action CreateSafeAction(Action action)
		{
			return () =>
			{
				try
				{
					action();
				}
				catch
				{
					CurrentTask.Abort();
				}
			};
		}

		/// <summary>
		///     Processes all remaining tasks. Call this periodically to allow the Dispatcher to handle dispatched tasks.
		///     Only call this inside the thread you want the tasks to process to be processed.
		/// </summary>
		public void ProcessTasks()
		{
			if (DataEvent.InterWaitOne(0))
			{
				ProcessTasksInternal();
			}
		}

		/// <summary>
		///     Processes all remaining tasks and returns true when something has been processed and false otherwise.
		///     This method will block until th exitHandle has been set or tasks should be processed.
		///     Only call this inside the thread you want the tasks to process to be processed.
		/// </summary>
		/// <param name="exitHandle">The handle to indicate an early abort of the wait process.</param>
		/// <returns>False when the exitHandle has been set, true otherwise.</returns>
		public bool ProcessTasks(WaitHandle exitHandle)
		{
			var result = WaitHandle.WaitAny(new [] {exitHandle, DataEvent});
			if (result == 0)
				return false;
			ProcessTasksInternal();
			return true;
		}

		/// <summary>
		///     Processed the next available task.
		///     Only call this inside the thread you want the tasks to process to be processed.
		/// </summary>
		/// <returns>True when a task to process has been processed, false otherwise.</returns>
		public bool ProcessNextTask()
		{
			CustomTask task;
			lock (TaskListSyncRoot)
			{
				if (TaskList.Count == 0)
					return false;
				task = TaskList.Dequeue();
			}

			ProcessSingleTask(task);

			if (TaskCount == 0)
				DataEvent.Reset();

			return true;
		}

		/// <summary>
		///     Processes the next available tasks and returns true when it has been processed and false otherwise.
		///     This method will block until th exitHandle has been set or a task should be processed.
		///     Only call this inside the thread you want the tasks to process to be processed.
		/// </summary>
		/// <param name="exitHandle">The handle to indicate an early abort of the wait process.</param>
		/// <returns>False when the exitHandle has been set, true otherwise.</returns>
		public bool ProcessNextTask(WaitHandle exitHandle)
		{
			var result = WaitHandle.WaitAny(new [] {exitHandle, DataEvent});
			if (result == 0)
				return false;

			CustomTask task;
			lock (TaskListSyncRoot)
			{
				if (TaskList.Count == 0)
					return false;
				task = TaskList.Dequeue();
			}

			ProcessSingleTask(task);
			if (TaskCount == 0)
				DataEvent.Reset();

			return true;
		}

		private void ProcessTasksInternal()
		{
			List<CustomTask> tmpCopy;
			lock (TaskListSyncRoot)
			{
				tmpCopy = new List<CustomTask>(TaskList);
				TaskList.Clear();
			}

			while (tmpCopy.Count != 0)
			{
				var task = tmpCopy[0];
				tmpCopy.RemoveAt(0);
				ProcessSingleTask(task);
			}

			if (TaskCount == 0)
				DataEvent.Reset();
		}

		private void ProcessSingleTask(CustomTask task)
		{
			RunTask(task);

			if (TaskSortingSystem == TaskSortingSystem.ReorderWhenExecuted)
				lock (TaskListSyncRoot)
					ReorderTasks();
		}

		internal void RunTask(CustomTask task)
		{
			var oldTask = _currentTask;
			_currentTask = task;
			_currentTask.DoInternal();
			_currentTask = oldTask;
		}

		/// <summary>
		/// Checks the access limitation.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Dispatching a Task with the Dispatcher associated to the current thread is prohibited. You can run these Tasks without the need of a Dispatcher.</exception>
		protected override void CheckAccessLimitation()
		{
			if (AllowAccessLimitationChecks && CurrentDispatcher == this)
				throw new InvalidOperationException(
					"Dispatching a Task with the Dispatcher associated to the current thread is prohibited. You can run these Tasks without the need of a Dispatcher.");
		}

		#region IDisposable Members

		/// <summary>
		///     Disposes all dispatcher resources and remaining tasks.
		/// </summary>
		public override void Dispose()
		{
			while (true)
			{
				lock (TaskListSyncRoot)
				{
					if (TaskList.Count != 0)
						_currentTask = TaskList.Dequeue();
					else
						break;
				}
				_currentTask.Dispose();
			}

			DataEvent.Close();
			DataEvent = null;

			if (CurrentDispatcher == this)
				CurrentDispatcher = null;
			if (MainDispatcher == this)
				MainDispatcher = null;
		}

		#endregion
	}
}