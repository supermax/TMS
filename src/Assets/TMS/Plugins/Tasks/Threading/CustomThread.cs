#region Code Editor

// Maxim

#endregion

#region

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using TMS.Common.Extensions;

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	public abstract class CustomThread : IDisposable
	{
		[ThreadStatic] private static CustomThread _currentThread;

		private readonly string _threadName;

		protected ManualResetEvent ExitEvent = new ManualResetEvent(false);

#if !WINDOWS_WP8 && !SILVERLIGHT && !UNITY_WP8
		private ThreadPriority _priority = ThreadPriority.BelowNormal;
#endif

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomThread" /> class.
		/// </summary>
		/// <param name="threadName">Name of the thread.</param>
		protected CustomThread(string threadName)
			: this(threadName, true)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomThread" /> class.
		/// </summary>
		/// <param name="threadName">Name of the thread.</param>
		/// <param name="autoStartThread">if set to <c>true</c> [automatic start thread].</param>
		protected CustomThread(string threadName, bool autoStartThread)
			: this(threadName, CustomTaskDispatcher.CurrentNoThrow, autoStartThread)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomThread" /> class.
		/// </summary>
		/// <param name="threadName">Name of the thread.</param>
		/// <param name="targetDispatcher">The target dispatcher.</param>
		protected CustomThread(string threadName, CustomTaskDispatcher targetDispatcher)
			: this(threadName, targetDispatcher, true)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="CustomThread" /> class.
		/// </summary>
		/// <param name="threadName">Name of the thread.</param>
		/// <param name="targetDispatcher">The target dispatcher.</param>
		/// <param name="autoStartThread">if set to <c>true</c> [automatic start thread].</param>
		protected CustomThread(string threadName, CustomTaskDispatcher targetDispatcher, bool autoStartThread)
		{
			_threadName = threadName;
			TargetDispatcher = targetDispatcher;

			if (autoStartThread)
			{
				Start();
			}
		}

		/// <summary>
		///     Gets or sets the target dispatcher.
		/// </summary>
		/// <value>
		///     The target dispatcher.
		/// </value>
		protected virtual CustomTaskDispatcher TargetDispatcher { get; set; }

		/// <summary>
		///     Gets or sets the thread.
		/// </summary>
		/// <value>
		///     The thread.
		/// </value>
		protected virtual Thread Thread { get; set; }

		/// <summary>
		///     Gets the available processors.
		/// </summary>
		/// <value>
		///     The available processors.
		/// </value>
		public static int AvailableProcessors
		{
			get
			{
#if UNITY3D
				return UnityEngine.SystemInfo.processorCount;
#else
				return Environment.ProcessorCount;
#endif
			}
		}

		/// <summary>
		///     Returns the currently ThreadBase instance which is running in this thread.
		/// </summary>
		public static CustomThread CurrentThread
		{
			get { return _currentThread; }
		}

		/// <summary>
		///     Returns true if the thread is working.
		/// </summary>
		public bool IsAlive
		{
			get { return Thread != null && Thread.IsAlive; }
		}

		/// <summary>
		///     Returns true if the thread should stop working.
		/// </summary>
		public bool ShouldStop
		{
			get { return ExitEvent.InterWaitOne(0); }
		}

#if !WINDOWS_WP8 && !SILVERLIGHT && !UNITY_WP8
		/// <summary>
		///     Gets or sets the priority.
		/// </summary>
		/// <value>
		///     The priority.
		/// </value>
		public ThreadPriority Priority
		{
			get { return _priority; }
			set
			{
				_priority = value;
				if (Thread != null)
					Thread.Priority = _priority;
			}
		}
#endif

		/// <summary>
		///     Starts the thread.
		/// </summary>
		public void Start()
		{
			if (Thread != null)
				Abort();

			ExitEvent.Reset();
			Thread = new Thread(DoInternal)
			{
				Name = _threadName,
#if !WINDOWS_WP8 && !SILVERLIGHT && !UNITY_WP8
				Priority = _priority,
#endif
			};

			Thread.Start();
		}

		/// <summary>
		///     Notifies the thread to stop working.
		/// </summary>
		public void Exit()
		{
			if (Thread != null)
				ExitEvent.Set();
		}

		/// <summary>
		///     Notifies the thread to stop working.
		/// </summary>
		public void Abort()
		{
			Exit();
			if (Thread != null)
				Thread.Join();
		}

		/// <summary>
		///     Notifies the thread to stop working and waits for completion for the given ammount of time.
		///     When the thread soes not stop after the given timeout the thread will be terminated.
		/// </summary>
		/// <param name="seconds">The time this method will wait until the thread will be terminated.</param>
		public void AbortWaitForSeconds(float seconds)
		{
			Exit();
			if (Thread == null) return;

			Thread.Join((int) (seconds*1000));
			if (Thread.IsAlive)
				Thread.Abort();
		}

		/// <summary>
		///     Creates a new Task for the target Dispatcher (default: the main Dispatcher) based upon the given function.
		/// </summary>
		/// <typeparam name="T">The return value of the task.</typeparam>
		/// <param name="function">The function to process at the dispatchers thread.</param>
		/// <returns>The new task.</returns>
		public CustomTask<T> Dispatch<T>(Func<T> function)
		{
			return TargetDispatcher.Dispatch(function);
		}

		/// <summary>
		///     Creates a new Task for the target Dispatcher (default: the main Dispatcher) based upon the given function.
		///     This method will wait for the task completion and returns the return value.
		/// </summary>
		/// <typeparam name="T">The return value of the task.</typeparam>
		/// <param name="function">The function to process at the dispatchers thread.</param>
		/// <returns>The return value of the tasks function.</returns>
		public T DispatchAndWait<T>(Func<T> function)
		{
			var task = Dispatch(function);
			task.Wait();
			return task.Result;
		}

		/// <summary>
		///     Creates a new Task for the target Dispatcher (default: the main Dispatcher) based upon the given function.
		///     This method will wait for the task completion or the timeout and returns the return value.
		/// </summary>
		/// <typeparam name="T">The return value of the task.</typeparam>
		/// <param name="function">The function to process at the dispatchers thread.</param>
		/// <param name="timeOutSeconds">Time in seconds after the waiting process will stop.</param>
		/// <returns>The return value of the tasks function.</returns>
		public T DispatchAndWait<T>(Func<T> function, float timeOutSeconds)
		{
			var task = Dispatch(function);
			task.WaitForSeconds(timeOutSeconds);
			return task.Result;
		}

		/// <summary>
		///     Creates a new Task for the target Dispatcher (default: the main Dispatcher) based upon the given action.
		/// </summary>
		/// <param name="action">The action to process at the dispatchers thread.</param>
		/// <returns>The new task.</returns>
		public CustomTask Dispatch(Action action)
		{
			return TargetDispatcher.Dispatch(action);
		}

		/// <summary>
		///     Creates a new Task for the target Dispatcher (default: the main Dispatcher) based upon the given action.
		///     This method will wait for the task completion.
		/// </summary>
		/// <param name="action">The action to process at the dispatchers thread.</param>
		public void DispatchAndWait(Action action)
		{
			var task = Dispatch(action);
			task.Wait();
		}

		/// <summary>
		///     Creates a new Task for the target Dispatcher (default: the main Dispatcher) based upon the given action.
		///     This method will wait for the task completion or the timeout.
		/// </summary>
		/// <param name="action">The action to process at the dispatchers thread.</param>
		/// <param name="timeOutSeconds">Time in seconds after the waiting process will stop.</param>
		public void DispatchAndWait(Action action, float timeOutSeconds)
		{
			var task = Dispatch(action);
			task.WaitForSeconds(timeOutSeconds);
		}

		/// <summary>
		///     Dispatches the given task to the target Dispatcher (default: the main Dispatcher).
		/// </summary>
		/// <param name="taskBase">The task to process at the dispatchers thread.</param>
		/// <returns>The new task.</returns>
		public CustomTask Dispatch(CustomTask taskBase)
		{
			return TargetDispatcher.Dispatch(taskBase);
		}

		/// <summary>
		///     Dispatches the given task to the target Dispatcher (default: the main Dispatcher).
		///     This method will wait for the task completion.
		/// </summary>
		/// <param name="taskBase">The task to process at the dispatchers thread.</param>
		public void DispatchAndWait(CustomTask taskBase)
		{
			var task = Dispatch(taskBase);
			task.Wait();
		}

		/// <summary>
		///     Dispatches the given task to the target Dispatcher (default: the main Dispatcher).
		///     This method will wait for the task completion or the timeout.
		/// </summary>
		/// <param name="taskBase">The task to process at the dispatchers thread.</param>
		/// <param name="timeOutSeconds">Time in seconds after the waiting process will stop.</param>
		public void DispatchAndWait(CustomTask taskBase, float timeOutSeconds)
		{
			var task = Dispatch(taskBase);
			task.WaitForSeconds(timeOutSeconds);
		}

		/// <summary>
		///     Does the internal.
		/// </summary>
		protected void DoInternal()
		{
			_currentThread = this;

			var enumerator = Do();
			if (enumerator == null)
			{
				return;
			}

			RunEnumerator(enumerator);
		}

		private void RunEnumerator(IEnumerator enumerator)
		{
			do
			{
				if (enumerator.Current is CustomTask)
				{
					var task = (CustomTask) enumerator.Current;
					DispatchAndWait(task);
				}
				else if (enumerator.Current is SwitchTo)
				{
					var switchTo = (SwitchTo) enumerator.Current;
					if (switchTo.Target == SwitchTo.TargetType.Main && CurrentThread != null)
					{
						var task = CustomTask.Create(() =>
						{
							if (enumerator.MoveNext() && !ShouldStop)
								RunEnumerator(enumerator);
						});
						DispatchAndWait(task);
					}
					else if (switchTo.Target == SwitchTo.TargetType.Thread && CurrentThread == null)
					{
						return;
					}
				}
			} while (enumerator.MoveNext() && !ShouldStop);
		}

		/// <summary>
		///     Does this instance.
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerator Do();

		#region IDisposable Members

		/// <summary>
		///     Disposes the thread and all resources.
		/// </summary>
		public virtual void Dispose()
		{
			AbortWaitForSeconds(1.0f);
		}

		#endregion
	}
}

//#if WINDOWS_WP8 || SILVERLIGHT
//namespace System.Threading
//{
//	/// <summary>
//	/// Specifies the scheduling priority of a <see cref="T:System.Threading.Thread"/>.
//	/// </summary>
//	/// <filterpriority>1</filterpriority>
//	[ComVisible(true)]
//	[Serializable]
//	public enum ThreadPriority
//	{
//		Lowest,
//		BelowNormal,
//		Normal,
//		AboveNormal,
//		Highest,
//	}
//}
//#endif