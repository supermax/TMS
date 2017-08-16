#region Code Editor

// Maxim

#endregion

#region

using System;
using System.Linq;
using System.Threading;

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	public class TaskDistributor : CustomTaskDispatcherBase
	{
		private readonly string _name;

		/// <summary>
		///     Amount of additional spawnable worker threads.
		/// </summary>
		public int MaxAdditionalWorkerThreads = 0;

#if !WINDOWS_WP8 && !SILVERLIGHT && !UNITY_WP8
		private ThreadPriority _priority = ThreadPriority.BelowNormal;
#endif

		private TaskWorker[] _workerThreads;

		/// <summary>
		///     Creates a new instance of the TaskDistributor with ProcessorCount x2 worker threads.
		///     The task distributor will auto start his worker threads.
		/// </summary>
		public TaskDistributor(string name)
			: this(name, 0)
		{
		}

		/// <summary>
		///     Creates a new instance of the TaskDistributor.
		///     The task distributor will auto start his worker threads.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="workerThreadCount">
		///     The number of worker threads, a value below one will create ProcessorCount x2 worker
		///     threads.
		/// </param>
		public TaskDistributor(string name, int workerThreadCount)
			: this(name, workerThreadCount, true)
		{
		}

		/// <summary>
		///     Creates a new instance of the TaskDistributor.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="workerThreadCount">
		///     The number of worker threads, a value below one will create ProcessorCount x2 worker
		///     threads.
		/// </param>
		/// <param name="autoStart">Should the instance auto start the worker threads.</param>
		public TaskDistributor(string name, int workerThreadCount, bool autoStart)
		{
			_name = name;
			if (workerThreadCount <= 0)
			{
				workerThreadCount = CustomThread.AvailableProcessors*2;
			}

			_workerThreads = new TaskWorker[workerThreadCount];
			lock (_workerThreads)
			{
				for (var i = 0; i < workerThreadCount; ++i)
					_workerThreads[i] = new TaskWorker(name, this);
			}

			if (MainNoThrow == null)
			{
				MainNoThrow = this;
			}

			if (autoStart)
			{
				Start();
			}
		}

		/// <summary>
		/// Gets the new data wait handle.
		/// </summary>
		/// <value>
		/// The new data wait handle.
		/// </value>
		internal WaitHandle NewDataWaitHandle
		{
			get { return DataEvent; }
		}

		/// <summary>
		///     Returns the first created TaskDistributor instance. When no instance has been created an exception will be thrown.
		/// </summary>
		public static TaskDistributor Main
		{
			get
			{
				if (MainNoThrow == null)
					throw new InvalidOperationException(
						"No default TaskDistributor found, please create a new TaskDistributor instance before calling this property.");

				return MainNoThrow;
			}
		}

		/// <summary>
		///     Returns the first created TaskDistributor instance.
		/// </summary>
		public static TaskDistributor MainNoThrow { get; private set; }

		/// <summary>
		///     Returns the currently existing task count. Early aborted tasks will count too.
		/// </summary>
		public override int TaskCount
		{
			get
			{
				var count = base.TaskCount;
				lock (_workerThreads)
				{
					count += _workerThreads.Sum(t => t.Dispatcher.TaskCount);
				}
				return count;
			}
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
				foreach (var worker in _workerThreads)
				{
					worker.Priority = value;
				}
			}
		}
#endif

		/// <summary>
		///     Starts the TaskDistributor if its not currently running.
		/// </summary>
		public void Start()
		{
			lock (_workerThreads)
			{
				foreach (var thread in _workerThreads)
				{
					if (!thread.IsAlive)
					{
						thread.Start();
					}
				}
			}
		}

		/// <summary>
		///     Spawns the additional worker thread.
		/// </summary>
		public void SpawnAdditionalWorkerThread()
		{
			lock (_workerThreads)
			{
				Array.Resize(ref _workerThreads, _workerThreads.Length + 1);
				_workerThreads[_workerThreads.Length - 1] = new TaskWorker(_name, this) 
#if !WINDOWS_WP8 && !SILVERLIGHT && !UNITY_WP8
				{Priority = _priority};
#else
					;
#endif
				_workerThreads[_workerThreads.Length - 1].Start();
			}
		}

		/// <summary>
		/// Fills the tasks.
		/// </summary>
		/// <param name="target">The target.</param>
		internal void FillTasks(CustomTaskDispatcher target)
		{
			target.AddTasks(IsolateTasks(1));
		}

		/// <summary>
		///     Checks the access limitation.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		///     Access to TaskDistributor prohibited when called from inside a
		///     TaskDistributor thread. Don't dispatch new Tasks through the same TaskDistributor. If you want to distribute new
		///     tasks create a new TaskDistributor and use the new created instance. Remember to dispose the new instance to
		///     prevent thread spamming.
		/// </exception>
		protected override void CheckAccessLimitation()
		{
			if (MaxAdditionalWorkerThreads > 0 || !AllowAccessLimitationChecks)
			{
				return;
			}

			if (CustomThread.CurrentThread is TaskWorker &&
			    ((TaskWorker) CustomThread.CurrentThread).TaskDistributor == this)
			{
				throw new InvalidOperationException(
					"Access to TaskDistributor prohibited when called from inside a TaskDistributor thread. Don't dispatch new Tasks through the same TaskDistributor. If you want to distribute new tasks create a new TaskDistributor and use the new created instance. Remember to dispose the new instance to prevent thread spamming.");
			}
		}

		internal override void TasksAdded()
		{
			if (MaxAdditionalWorkerThreads > 0 &&
			    (_workerThreads.All(worker => worker.Dispatcher.TaskCount > 0 || worker.IsWorking) ||
			     TaskList.Count > _workerThreads.Length))
			{
				Interlocked.Decrement(ref MaxAdditionalWorkerThreads);
				SpawnAdditionalWorkerThread();
			}

			base.TasksAdded();
		}

		#region IDisposable Members

		private bool _isDisposed;

		/// <summary>
		///     Disposes all TaskDistributor, worker threads, resources and remaining tasks.
		/// </summary>
		public override void Dispose()
		{
			if (_isDisposed) return;

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

			lock (_workerThreads)
			{
				foreach (var thread in _workerThreads)
				{
					thread.Dispose();
				}
				_workerThreads = new TaskWorker[0];
			}

			DataEvent.Close();
			DataEvent = null;

			if (MainNoThrow == this)
				MainNoThrow = null;

			_isDisposed = true;
		}

		#endregion
	}
}