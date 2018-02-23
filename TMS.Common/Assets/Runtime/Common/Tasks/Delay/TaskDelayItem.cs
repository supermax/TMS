using System;
using System.Threading;

namespace TMS.Common.Tasks.Delay
{
	/// <summary>
	/// Task Delay Item
	/// </summary>
	public class TaskDelayItem : IDisposable
	{
		private ManualResetEvent _manualResetEvent;

		/// <summary>
		/// Gets the id.
		/// </summary>
		/// <value>
		/// The id.
		/// </value>
		public Guid Id { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskDelayItem"/> class.
		/// </summary>
		public TaskDelayItem()
		{
			_manualResetEvent = new ManualResetEvent(false);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TaskDelayItem"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		public TaskDelayItem(Guid id)
			: this()
		{
			Id = id;
		}

		/// <summary>
		/// Delays the specified milliseconds.
		/// </summary>
		/// <param name="span">The span.</param>
		/// <returns></returns>
		public bool Delay(TimeSpan span)
		{
			_manualResetEvent.Reset();
			var abort = _manualResetEvent.WaitOne(span);
			IsAborted = abort;
			IsFinished = true;
			return abort;
		}

		/// <summary>
		/// Gets a value indicating whether this instance is aborted.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is aborted; otherwise, <c>false</c>.
		/// </value>
		public bool IsAborted { get; private set;}

		/// <summary>
		/// Gets a value indicating whether this instance is finished.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is finished; otherwise, <c>false</c>.
		/// </value>
		public bool IsFinished { get; private set;}

		/// <summary>
		/// Aborts this instance.
		/// </summary>
		/// <returns></returns>
		public bool Abort()
		{
			IsAborted = true;
			var res = _manualResetEvent.Set();
			return res;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_manualResetEvent == null) return;

			_manualResetEvent.Set();
			//if (_manualResetEvent == null) return;

			//_manualResetEvent.Dispose();
			_manualResetEvent = null;
		}
	}
}