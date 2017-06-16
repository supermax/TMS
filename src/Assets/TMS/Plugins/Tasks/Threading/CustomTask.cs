#region

using System;
using System.Collections;
using System.Threading;
using TMS.Common.Extensions;

#if UNITY3D
using UnityEngine;
#else
using System.Diagnostics;
#endif

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	public abstract class CustomTask
	{
		[ThreadStatic] private static CustomTask _current;
		private readonly ManualResetEvent _abortEvent = new ManualResetEvent(false);
		private readonly ManualResetEvent _endedEvent = new ManualResetEvent(false);
		private readonly ManualResetEvent _endingEvent = new ManualResetEvent(false);
		private readonly object _syncRoot = new object();

		private bool _hasEnded;

		private bool _hasStarted;
		private bool _isDisposed;

		/// <summary>
		///     Gets or sets the name.
		/// </summary>
		/// <value>
		///     The name.
		/// </value>
		public virtual string Name { get; set; }

		/// <summary>
		///     Change this when you work with a prioritizable Dispatcher or TaskDistributor to change the execution order
		///     lower values will be executed first.
		/// </summary>
		public int Priority { get; set; }

		/// <summary>
		///     Returns the currently ThreadBase instance which is running in this thread.
		/// </summary>
		public static CustomTask Current
		{
			get { return _current; }
		}

		/// <summary>
		///     Returns true if the task should abort. If a Task should abort and has not yet been started
		///     it will never start but indicate an end and failed state.
		/// </summary>
		public bool ShouldAbort
		{
			get { return _abortEvent.InterWaitOne(0); }
		}

		/// <summary>
		///     Returns true when processing of this task has been ended or has been skipped due early abortion.
		/// </summary>
		public bool HasEnded
		{
			get { return _hasEnded || _endedEvent.InterWaitOne(0); }
		}

		/// <summary>
		///     Returns true when processing of this task is ending.
		/// </summary>
		public bool IsEnding
		{
			get { return _endingEvent.InterWaitOne(0); }
		}


		/// <summary>
		///     Returns true when the task has successfully been processed. Tasks which throw exceptions will
		///     not be set to a failed state, also any exceptions will not be caught, the user needs to add
		///     checks for these kind of situation.
		/// </summary>
		public bool IsSucceeded
		{
			get { return _endingEvent.InterWaitOne(0) && !_abortEvent.InterWaitOne(0); }
		}

		/// <summary>
		///     Returns true if the task should abort and has been ended. This value will not been set to true
		///     in case of an exception while processing this task. The user needs to add checks for these kind of situation.
		/// </summary>
		public bool IsFailed
		{
			get { return _endingEvent.InterWaitOne(0) && _abortEvent.InterWaitOne(0); }
		}

		/// <summary>
		///     Gets the raw result.
		/// </summary>
		/// <value>
		///     The raw result.
		/// </value>
		public abstract object RawResult { get; }

		/// <summary>
		///     Finalizes an instance of the <see cref="CustomTask" /> class.
		/// </summary>
		~CustomTask()
		{
			Dispose();
		}

		private event TaskEndedEventHandler taskEnded;

		/// <summary>
		///     Will be called when the task has been finished (success or failure or aborted).
		///     This event will be fired at the thread the task was running at.
		/// </summary>
		public event TaskEndedEventHandler TaskEnded
		{
			add
			{
				lock (_syncRoot)
				{
					if (_endingEvent.InterWaitOne(0))
					{
						value(this);
						return;
					}
					taskEnded += value;
				}
			}
			remove
			{
				lock (_syncRoot)
					taskEnded -= value;
			}
		}

		private void End()
		{
			lock (_syncRoot)
			{
				_endingEvent.Set();

				if (taskEnded != null)
					taskEnded(this);

				_endedEvent.Set();
				if (_current == this)
					_current = null;
				_hasEnded = true;
			}
		}

		/// <summary>
		///     Does this instance.
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerator Do();

		/// <summary>
		///     Notifies the task to abort and sets the task state to failed. The task needs to check ShouldAbort if the task
		///     should abort.
		/// </summary>
		public void Abort()
		{
			_abortEvent.Set();
			if (!_hasStarted)
			{
				End();
			}
		}

		/// <summary>
		///     Notifies the task to abort and sets the task state to failed. The task needs to check ShouldAbort if the task
		///     should abort.
		///     This method will wait until the task has been aborted/ended.
		/// </summary>
		public void AbortWait()
		{
			Abort();
			if (!_hasStarted)
				return;
			Wait();
		}

		/// <summary>
		///     Notifies the task to abort and sets the task state to failed. The task needs to check ShouldAbort if the task
		///     should abort.
		///     This method will wait until the task has been aborted/ended or the given timeout has been reached.
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
		public void AbortWaitForSeconds(float seconds)
		{
			Abort();
			if (!_hasStarted) return;

			WaitForSeconds(seconds);
		}

		/// <summary>
		///     Blocks the calling thread until the task has been ended.
		/// </summary>
		public void Wait()
		{
			if (_hasEnded) return;

			Priority--;
			_endedEvent.WaitOne();
		}

		/// <summary>
		///     Blocks the calling thread until the task has been ended or the given timeout value has been reached.
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
		public void WaitForSeconds(float seconds)
		{
			if (_hasEnded) return;

			Priority--;
			_endedEvent.InterWaitOne(TimeSpan.FromSeconds(seconds));
		}

		/// <summary>
		///     Blocks the calling thread until the task has been ended and returns the return value of the task as the given type.
		///     Use this method only for Tasks with return values (functions)!
		/// </summary>
		/// <returns>The return value of the task as the given type.</returns>
		public abstract TResult Wait<TResult>();

		/// <summary>
		///     Blocks the calling thread until the task has been ended and returns the return value of the task as the given type.
		///     Use this method only for Tasks with return values (functions)!
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
		/// <returns>The return value of the task as the given type.</returns>
		public abstract TResult WaitForSeconds<TResult>(float seconds);

		/// <summary>
		///     Blocks the calling thread until the task has been ended and returns the return value of the task as the given type.
		///     Use this method only for Tasks with return values (functions)!
		/// </summary>
		/// <param name="seconds">Time in seconds this method will max wait.</param>
		/// <param name="defaultReturnValue">The default return value which will be returned when the task has failed.</param>
		/// <returns>The return value of the task as the given type.</returns>
		public abstract TResult WaitForSeconds<TResult>(float seconds, TResult defaultReturnValue);

		internal void DoInternal()
		{
			_current = this;
			_hasStarted = true;
			if (!ShouldAbort)
			{
				try
				{
					var enumerator = Do();
					if (enumerator == null)
					{
						End();
						return;
					}

					RunEnumerator(enumerator);
				}
				catch (Exception exception) // TODO handle exc
				{
					Abort();
#if UNITY3D
					if (string.IsNullOrEmpty(Name))
						UnityEngine.Debug.LogError("Error while processing task:\n" + exception.ToString());
					else
						UnityEngine.Debug.LogError("Error while processing task '" + Name + "':\n" + exception.ToString());
#else
					Debug.WriteLine(exception);
#endif
				}
			}

			End();
		}

		private void RunEnumerator(IEnumerator enumerator)
		{
			var currentThread = CustomThread.CurrentThread;
			do
			{
				if (enumerator.Current is CustomTask)
				{
					var task = (CustomTask) enumerator.Current;
					currentThread.DispatchAndWait(task);
				}
				else if (enumerator.Current is SwitchTo)
				{
					var switchTo = (SwitchTo) enumerator.Current;
					if (switchTo.Target == SwitchTo.TargetType.Main && currentThread != null)
					{
						var task = Create(() =>
						{
							if (enumerator.MoveNext() && !ShouldAbort)
								RunEnumerator(enumerator);
						});
						currentThread.DispatchAndWait(task);
					}
					else if (switchTo.Target == SwitchTo.TargetType.Thread && currentThread == null)
					{
						return;
					}
				}
			} while (enumerator.MoveNext() && !ShouldAbort);
		}

		/// <summary>
		///     Disposes this task and waits for completion if its still running.
		/// </summary>
		public void Dispose()
		{
			if (_isDisposed) return;
			_isDisposed = true;

			if (_hasStarted)
			{
				Wait();
			}

			_endingEvent.Close();
			_endedEvent.Close();
			_abortEvent.Close();
		}

		/// <summary>
		///     Starts the task on the given DispatcherBase (Dispatcher or TaskDistributor).
		/// </summary>
		/// <param name="target">The DispatcherBase to work on.</param>
		/// <returns>This task</returns>
		public CustomTask Run(CustomTaskDispatcherBase target)
		{
			if (target == null)
			{
				return Run();
			}

			target.Dispatch(this);
			return this;
		}

		/// <summary>
		///     Starts the task.
		/// </summary>
		/// <returns>This task</returns>
		public CustomTask Run()
		{
			Run(ThreadHelper.TaskDistributor);
			return this;
		}

		/// <summary>
		///     Creates the specified action.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		public static CustomTask Create(Action<CustomTask> action)
		{
			return new CustomTask<Unit>(action);
		}

		/// <summary>
		///     Creates the specified action.
		/// </summary>
		/// <param name="action">The action.</param>
		/// <returns></returns>
		public static CustomTask Create(Action action)
		{
			return new CustomTask<Unit>(action);
		}

		/// <summary>
		///     Creates the specified function.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func">The function.</param>
		/// <returns></returns>
		public static CustomTask<T> Create<T>(Func<CustomTask, T> func)
		{
			return new CustomTask<T>(func);
		}

		/// <summary>
		///     Creates the specified function.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="func">The function.</param>
		/// <returns></returns>
		public static CustomTask<T> Create<T>(Func<T> func)
		{
			return new CustomTask<T>(func);
		}

		/// <summary>
		///     Creates the specified enumerator.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		/// <returns></returns>
		public static CustomTask Create(IEnumerator enumerator)
		{
			return new CustomTask<IEnumerator>(enumerator);
		}

		/// <summary>
		///     Creates the specified type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="type">The type.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public static CustomTask<T> Create<T>(Type type, string methodName, params object[] args)
		{
			return new CustomTask<T>(type, methodName, args);
		}

		/// <summary>
		///     Creates the specified that.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="that">The that.</param>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public static CustomTask<T> Create<T>(object that, string methodName, params object[] args)
		{
			return new CustomTask<T>(that, methodName, args);
		}

		/// <summary>
		///     Empty Struct which works as the Void type.
		/// </summary>
		public struct Unit
		{
			public static Unit Default = new Unit();
		}
	}
}