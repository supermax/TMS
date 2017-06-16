#region

using System;
using System.Collections.Generic;
using TMS.Common.Extensions;
using TMS.Common.Messaging;

#endregion

namespace TMS.Common.Tasks.Timeout
{
	[MessengerConsumer(typeof(ITimeoutDelegateManager), IsSingleton = true, InstantiateOnRegistration = true)]
	internal class TimeoutDelegateManager : ITimeoutDelegateManager, IMessengerConsumer, IDisposable
	{
		internal const double QueueTimerInterval = 1000.0; // TODO from config
		private readonly object _locker = new object();
		private bool _isDisposed;
		private IList<TimeoutDelegatePayload> _items;
		
		private UnityTimer _timer;

		public UnityTimer Timer
		{
			get
			{
				var res = _locker.InitWithLock(ref _timer, () =>
				{
					var timer = UnityTimer.CreateTimer(OnTimerTick, TimeSpan.FromMilliseconds(QueueTimerInterval));
					return timer;
				});
				return res;
			}
		}

		protected IList<TimeoutDelegatePayload> Items
		{
			get
			{
				var res = _locker.InitWithLock(ref _items, () => new List<TimeoutDelegatePayload>());
				return res;
			}
		}
		
		/// <summary>
		///     Subscribes this instance.
		/// </summary>
		public void Subscribe()
		{
			Messenger.Default.Subscribe<TimeoutDelegateCancelPayload>(OnTimeoutCancelation);
			Messenger.Default.Subscribe<TimeoutDelegatePayload>(OnTimeoutRegistration);
		}

		private void OnTimeoutRegistration(TimeoutDelegatePayload arg)
		{
			try
			{
				ArgumentValidator.AssertNotNull(arg, "arg");
				ArgumentValidator.AssertNotNull(arg.Method, "arg.Method");

				arg.TimeoutStartTime = DateTime.Now;
				Items.Add(arg);
			}
			finally
			{
				if (Items.Count > 0)
				{
					Timer.StartTimer();
				}
			}
		}

		private void OnTimeoutCancelation(TimeoutDelegateCancelPayload arg)
		{
			try
			{
				ArgumentValidator.AssertNotNull(arg, "arg");
				ArgumentValidator.AssertNotNull(arg.TimeoutPayload, "arg.TimeoutPayload");
				
				if (Items.Contains(arg.TimeoutPayload))
				{
					Items.Remove(arg.TimeoutPayload);
					arg.Dispose();
				}
				arg.Invoke();
			}
			finally
			{
				if (Items.Count < 1)
				{
					Timer.StopTimer();
				}
			}
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				throw new OperationCanceledException("This instance is disposed already!");
			}
			_isDisposed = true;

			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}

			if (_items == null) return;
			foreach (var item in _items)
			{
				item.Dispose();
			}
			_items.Clear();
			_items = null;
		}

		private void OnTimerTick()
		{
			try
			{
				var list = new List<TimeoutDelegatePayload>();
				foreach (var timeoutPayload in Items)
				{
					if (timeoutPayload.IsTimeoutOccured())
					{
						list.Add(timeoutPayload);
					}
				}
				var timeoutedItems = list.ToArray();
				if (timeoutedItems.IsNullOrEmpty()) return;

				foreach (var timeoutPayload in timeoutedItems)
				{
					try
					{
						timeoutPayload.Invoke();
					}
					finally
					{
						Items.Remove(timeoutPayload);
						timeoutPayload.Dispose();
					}
				}
			}
			finally
			{
				if (Items.Count < 1)
				{
					Timer.StopTimer();
				}
			}
		}
	}
}