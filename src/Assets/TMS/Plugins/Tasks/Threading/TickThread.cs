#region Code Editor

// Maxim

#endregion

#region

using System;
using System.Collections;
using System.Threading;
using TMS.Common.Extensions;

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	public sealed class TickThread : CustomThread
	{
		private readonly Action _action;
		private readonly ManualResetEvent _tickEvent = new ManualResetEvent(false);
		private readonly int _tickLengthInMilliseconds;

		/// <summary>
		///     Creates a new Thread which runs the given action.
		///     The thread will start running after creation.
		/// </summary>
		/// <param name="action">The enumerable action to run.</param>
		/// <param name="tickLengthInMilliseconds">Time between ticks.</param>
		public TickThread(Action action, int tickLengthInMilliseconds)
			: this(action, tickLengthInMilliseconds, true)
		{
		}

		/// <summary>
		///     Creates a new Thread which runs the given action.
		/// </summary>
		/// <param name="action">The action to run.</param>
		/// <param name="tickLengthInMilliseconds">Time between ticks.</param>
		/// <param name="autoStartThread">Should the thread start after creation.</param>
		public TickThread(Action action, int tickLengthInMilliseconds, bool autoStartThread)
			: base("TickThread", CustomTaskDispatcher.CurrentNoThrow, false)
		{
			_tickLengthInMilliseconds = tickLengthInMilliseconds;
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
			while (!ExitEvent.InterWaitOne(0))
			{
				_action();

				var result = WaitHandle.WaitAny(new WaitHandle[] {ExitEvent, _tickEvent}, _tickLengthInMilliseconds);
				if (result == 0)
					return null;
			}
			return null;
		}
	}
}