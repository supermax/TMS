#region Code Editor

// Maxim

#endregion

#region

using System;
using System.Collections.Generic;
using System.Threading;

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Channel<T> : IDisposable
	{
		private readonly List<T> _buffer = new List<T>();
		private readonly object _disposeRoot = new object();
		private readonly object _getSyncRoot = new object();
		private readonly object _setSyncRoot = new object();
		private ManualResetEvent _exitEvent = new ManualResetEvent(false);
		private ManualResetEvent _getEvent = new ManualResetEvent(true);
		private bool _isDisposed;
		private ManualResetEvent _setEvent = new ManualResetEvent(false);

		/// <summary>
		///     Initializes a new instance of the <see cref="Channel{T}" /> class.
		/// </summary>
		public Channel()
			: this(1)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="Channel{T}" /> class.
		/// </summary>
		/// <param name="bufferSize">Size of the buffer.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">bufferSize;Must be greater or equal to 1.</exception>
		public Channel(int bufferSize)
		{
			if (bufferSize < 1)
				throw new ArgumentOutOfRangeException("bufferSize", "Must be greater or equal to 1.");

			BufferSize = bufferSize;
		}

		/// <summary>
		///     Gets the size of the buffer.
		/// </summary>
		/// <value>
		///     The size of the buffer.
		/// </value>
		public int BufferSize { get; private set; }

		~Channel()
		{
			Dispose();
		}

		/// <summary>
		///     Resizes the specified new buffer size.
		/// </summary>
		/// <param name="newBufferSize">New size of the buffer.</param>
		/// <exception cref="System.ArgumentOutOfRangeException">newBufferSize;Must be greater or equal to 1.</exception>
		public void Resize(int newBufferSize)
		{
			if (newBufferSize < 1)
				throw new ArgumentOutOfRangeException("newBufferSize", "Must be greater or equal to 1.");

			lock (_setSyncRoot)
			{
				if (_isDisposed) return;

				var result = WaitHandle.WaitAny(new WaitHandle[] {_exitEvent, _getEvent});
				if (result == 0) return;

				_buffer.Clear();

				BufferSize = newBufferSize;
			}
		}

		/// <summary>
		///     Sets the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public bool Set(T value)
		{
			return Set(value, int.MaxValue);
		}

		/// <summary>
		///     Sets the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="timeoutInMilliseconds">The timeout in milliseconds.</param>
		/// <returns></returns>
		public bool Set(T value, int timeoutInMilliseconds)
		{
			lock (_setSyncRoot)
			{
				if (_isDisposed)
					return false;

				var result = WaitHandle.WaitAny(new WaitHandle[] {_exitEvent, _getEvent}, timeoutInMilliseconds);
				if (result == WaitHandle.WaitTimeout || result == 0)
					return false;

				_buffer.Add(value);
				if (_buffer.Count != BufferSize) return true;

				_setEvent.Set();
				_getEvent.Reset();

				return true;
			}
		}

		/// <summary>
		///     Gets this instance.
		/// </summary>
		/// <returns></returns>
		public T Get()
		{
			return Get(int.MaxValue, default(T));
		}

		/// <summary>
		///     Gets the specified timeout in milliseconds.
		/// </summary>
		/// <param name="timeoutInMilliseconds">The timeout in milliseconds.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns></returns>
		public T Get(int timeoutInMilliseconds, T defaultValue)
		{
			lock (_getSyncRoot)
			{
				if (_isDisposed)
					return defaultValue;

				var result = WaitHandle.WaitAny(new WaitHandle[] {_exitEvent, _setEvent}, timeoutInMilliseconds);
				if (result == WaitHandle.WaitTimeout || result == 0)
					return defaultValue;

				var value = _buffer[0];
				_buffer.RemoveAt(0);
				if (_buffer.Count == 0)
				{
					_getEvent.Set();
					_setEvent.Reset();
				}

				return value;
			}
		}

		/// <summary>
		///     Closes this instance.
		/// </summary>
		public void Close()
		{
			lock (_disposeRoot)
			{
				if (_isDisposed)
					return;

				_exitEvent.Set();
			}
		}

		#region IDisposable Members

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			if (_isDisposed)
				return;

			lock (_disposeRoot)
			{
				_exitEvent.Set();

				lock (_getSyncRoot)
				{
					lock (_setSyncRoot)
					{
						_setEvent.Close();
						_setEvent = null;

						_getEvent.Close();
						_getEvent = null;

						_exitEvent.Close();
						_exitEvent = null;

						_isDisposed = true;
					}
				}
			}
		}

		#endregion
	}

	/// <summary>
	/// </summary>
	public class Channel : Channel<object>
	{
	}
}