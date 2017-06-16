#region Code Editor

// Maxim

#endregion

#region

using System;
using System.Collections;

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	public sealed class ActionThread : CustomThread
	{
		private readonly Action<ActionThread> _action;

		/// <summary>
		///     Creates a new Thread which runs the given action.
		///     The thread will start running after creation.
		/// </summary>
		/// <param name="action">The action to run.</param>
		public ActionThread(Action<ActionThread> action)
			: this(action, true)
		{
		}

		/// <summary>
		///     Creates a new Thread which runs the given action.
		/// </summary>
		/// <param name="action">The action to run.</param>
		/// <param name="autoStartThread">Should the thread start after creation.</param>
		public ActionThread(Action<ActionThread> action, bool autoStartThread)
			: base("ActionThread", CustomTaskDispatcher.Current, false)
		{
			_action = action;

			if (autoStartThread)
			{
				Start();
			}
		}

		/// <summary>
		///     Does this instance.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerator Do()
		{
			_action(this);
			return null;
		}
	}
}