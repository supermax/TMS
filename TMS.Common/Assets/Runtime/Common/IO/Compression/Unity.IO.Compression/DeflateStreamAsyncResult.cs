#region Usings

using System;
using System.Threading;

#endregion

#if !NETFX_CORE

namespace Unity.IO.Compression
{
	internal class DeflateStreamAsyncResult : IAsyncResult
	{
		public byte[] buffer;
		public int count;
		// disable csharp compiler warning #0414: field assigned unused value
#pragma warning disable 0414
		public bool isWrite;
#pragma warning restore 0414
		private readonly AsyncCallback m_AsyncCallback; // Caller's callback method.

		private object m_AsyncObject; // Caller's async object.
		private int m_Completed; // 0 if not completed >0 otherwise.
		internal bool m_CompletedSynchronously; // true if the operation completed synchronously.
		private object m_Event; // lazy allocated event to be returned in the IAsyncResult for the client to wait on
		private int m_InvokedCallback; // 0 is callback is not called

		public int offset;

		public DeflateStreamAsyncResult(object asyncObject, object asyncState,
			AsyncCallback asyncCallback,
			byte[] buffer, int offset, int count)
		{
			this.buffer = buffer;
			this.offset = offset;
			this.count = count;
			m_CompletedSynchronously = true;
			m_AsyncObject = asyncObject;
			AsyncState = asyncState;
			m_AsyncCallback = asyncCallback;
		}

		// Internal property for setting the IO result.
		internal object Result { get; private set; }

		// Interface method to return the caller's state object.
		public object AsyncState { get; private set; }

		// Interface property to return a WaitHandle that can be waited on for I/O completion.
		// This property implements lazy event creation.
		// the event object is only created when this property is accessed,
		// since we're internally only using callbacks, as long as the user is using
		// callbacks as well we will not create an event at all.
		public WaitHandle AsyncWaitHandle
		{
			get
			{
				// save a copy of the completion status
				var savedCompleted = m_Completed;
				if (m_Event == null) Interlocked.CompareExchange(ref m_Event, new ManualResetEvent(savedCompleted != 0), null);

				var castedEvent = (ManualResetEvent) m_Event;
				if (savedCompleted == 0 && m_Completed != 0) castedEvent.Set();
				return castedEvent;
			}
		}

		// Interface property, returning synchronous completion status.
		public bool CompletedSynchronously
		{
			get { return m_CompletedSynchronously; }
		}

		// Interface property, returning completion status.
		public bool IsCompleted
		{
			get { return m_Completed != 0; }
		}

		internal void Close()
		{
			if (m_Event != null) ((ManualResetEvent) m_Event).Close();
		}

		internal void InvokeCallback(bool completedSynchronously, object result)
		{
			Complete(completedSynchronously, result);
		}

		internal void InvokeCallback(object result)
		{
			Complete(result);
		}

		// Internal method for setting completion.
		// As a side effect, we'll signal the WaitHandle event and clean up.
		private void Complete(bool completedSynchronously, object result)
		{
			m_CompletedSynchronously = completedSynchronously;
			Complete(result);
		}

		private void Complete(object result)
		{
			Result = result;

			// Set IsCompleted and the event only after the usercallback method. 
			Interlocked.Increment(ref m_Completed);

			if (m_Event != null) ((ManualResetEvent) m_Event).Set();

			if (Interlocked.Increment(ref m_InvokedCallback) == 1) if (m_AsyncCallback != null) m_AsyncCallback(this);
		}
	}
}

#endif