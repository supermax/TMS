#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMS.Common.Core;
#if UNITY3D
using UnityEngine;
#else
using System.Diagnostics;
#endif

#endregion

namespace TMS.Common.Tasks.Threading
{
#if UNITY3D
	/// <summary>
	/// http://forum.unity3d.com/threads/unity-threading-helper.90128/
	/// </summary>
//[ExecuteInEditMode]
public class ThreadHelper : MonoBehaviorBaseSingleton<ThreadHelper>
#else
	/// <summary>
	/// </summary>
	public class ThreadHelper : Singleton<ThreadHelper>
#endif
	{
		private void Init()
	    {
            _mainThreadId = Thread.CurrentThread.ManagedThreadId;
            CurrentDispatcher = CustomTaskDispatcher.MainNoThrow ?? new CustomTaskDispatcher();
            CurrentTaskDistributor = TaskDistributor.MainNoThrow ?? new TaskDistributor("TaskDistributor");
        }

		/// <summary>
		///     Returns the GUI/Main Dispatcher.
		/// </summary>
		public static CustomTaskDispatcher Dispatcher
		{
			get { return Default.CurrentDispatcher; }
		}

		/// <summary>
		///     Returns the TaskDistributor.
		/// </summary>
		public static TaskDistributor TaskDistributor
		{
			get { return Default.CurrentTaskDistributor; }
		}

		/// <summary>
		///     Gets the current dispatcher.
		/// </summary>
		/// <value>
		///     The current dispatcher.
		/// </value>
		public CustomTaskDispatcher CurrentDispatcher { get; private set; }

		/// <summary>
		///     Gets the current task distributor.
		/// </summary>
		/// <value>
		///     The current task distributor.
		/// </value>
		public TaskDistributor CurrentTaskDistributor { get; private set; }

		private int _mainThreadId;

        /// <summary>
        /// Determines whether called method is on main thread
        /// </summary>
        /// <returns></returns>
        public bool IsMainThread()
	    {
	        var isMain = _mainThreadId == Thread.CurrentThread.ManagedThreadId;
	        return isMain;
	    }

		/// <summary>
		///     Creates new thread which runs the given action. The given action will be wrapped so that any exception will be
		///     caught and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ActionThread CreateThread(Action<ActionThread> action, bool autoStartThread)
		{
		    var instance = Default;
            Action<ActionThread> actionWrapper = currentThread =>
			{
				try
				{
					action(currentThread);
				}
				catch (Exception ex) // TODO handle exc
				{
#if UNITY3D
					UnityEngine.Debug.LogError(ex);
#else
					Debug.WriteLine(ex);
#endif
				}
			};

			var thread = new ActionThread(actionWrapper, autoStartThread);
            instance.RegisterThread(thread);
			return thread;
		}

		/// <summary>
		///     Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so
		///     that any exception will be caught and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ActionThread CreateThread(Action<ActionThread> action)
		{
			return CreateThread(action, true);
		}

		/// <summary>
		///     Creates new thread which runs the given action. The given action will be wrapped so that any exception will be
		///     caught and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ActionThread CreateThread(Action action, bool autoStartThread)
		{
			return CreateThread(thread => action(), autoStartThread);
		}

		/// <summary>
		///     Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so
		///     that any exception will be caught and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static ActionThread CreateThread(Action action)
		{
			return CreateThread(thread => action(), true);
		}

		#region Enumerable

		/// <summary>
		///     Creates new thread which runs the given action. The given action will be wrapped so that any exception will be
		///     caught and logged.
		/// </summary>
		/// <param name="action">The enumerable action which the new thread should run.</param>
		/// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static CustomThread CreateThread(Func<CustomThread, IEnumerator> action, bool autoStartThread)
		{
		    var instance = Default;
            var thread = new EnumeratableActionThread(action, autoStartThread);
            instance.RegisterThread(thread);
			return thread;
		}

		/// <summary>
		///     Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so
		///     that any exception will be caught and logged.
		/// </summary>
		/// <param name="action">The enumerable action which the new thread should run.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static CustomThread CreateThread(Func<CustomThread, IEnumerator> action)
		{
			return CreateThread(action, true);
		}

		/// <summary>
		///     Creates new thread which runs the given action. The given action will be wrapped so that any exception will be
		///     caught and logged.
		/// </summary>
		/// <param name="action">The enumerable action which the new thread should run.</param>
		/// <param name="autoStartThread">True when the thread should start immediately after creation.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static CustomThread CreateThread(Func<IEnumerator> action, bool autoStartThread)
		{
			Func<CustomThread, IEnumerator> wrappedAction = thread => action();
			return CreateThread(wrappedAction, autoStartThread);
		}

		/// <summary>
		///     Creates new thread which runs the given action and starts it after creation. The given action will be wrapped so
		///     that any exception will be caught and logged.
		/// </summary>
		/// <param name="action">The action which the new thread should run.</param>
		/// <returns>The instance of the created thread class.</returns>
		public static CustomThread CreateThread(Func<IEnumerator> action)
		{
			Func<CustomThread, IEnumerator> wrappedAction = thread => action();
			return CreateThread(wrappedAction, true);
		}

		#endregion

		private readonly List<CustomThread> _registeredThreads = new List<CustomThread>();

		/// <summary>
		///     Registers the thread.
		/// </summary>
		/// <param name="thread">The thread.</param>
		private void RegisterThread(CustomThread thread)
		{
			if (_registeredThreads.Contains(thread))
			{
				return;
			}

			_registeredThreads.Add(thread);
		}

#if UNITY3D
		protected override void Awake()
		{
			Init();
			base.Awake();
		}

		protected override void OnDestroy()
		{
			foreach (var thread in _registeredThreads)
				thread.Dispose();

			if (CurrentDispatcher != null)
				CurrentDispatcher.Dispose();
			CurrentDispatcher = null;

			if (CurrentTaskDistributor != null)
				CurrentTaskDistributor.Dispose();
			CurrentTaskDistributor = null;

			base.OnDestroy();
		}

		protected virtual void Update()
		{
			if (CurrentDispatcher != null)
			{
				CurrentDispatcher.ProcessTasks();
			}

			foreach (var thread in _registeredThreads.ToArray())
			{
				if (thread.IsAlive) continue;
				thread.Dispose();
				_registeredThreads.Remove(thread);
			}
		}
#else
		public ThreadHelper()
		{
			Init();
		}
#endif
	}
}