#region

using System;
using TMS.Common.Core;
using TMS.Common.Extensions;
using UnityEngine;

#endregion

namespace TMS.Common.Tasks.Timeout
{
	public class UnityTimer : MonoBehaviourBase
	{
		public bool IsRepeating;
		public TimeSpan TimeInterval = new TimeSpan(0, 0, 0, 1, 0);
		protected Action TimerCallBack;
		protected static string OnTickMethodName;
		public bool Autorun;

		public bool IsRunning { get; protected set; }

		public static UnityTimer CreateTimer(Action timerCallback, TimeSpan timeSpan, bool isRepeating = true, bool autorun = true)
		{
			ArgumentValidator.AssertNotNull(timerCallback, "timerCallback");
			if (timeSpan == TimeSpan.Zero)
			{
				throw new ArgumentException("wrong timeSpan");
			}

			var go = new GameObject(string.Format("_timerHost_{0}_{1}", timerCallback, timerCallback.GetHashCode()))
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			var timer = go.AddComponent<UnityTimer>();
			timer.TimeInterval = timeSpan;
			timer.IsRepeating = isRepeating;
			timer.TimerCallBack = timerCallback;
			timer.Autorun = autorun;

			//Debug.LogWarning(string.Format("<<< CreateTimer ({0}, {1}, {2}) >>>", autorun, isRepeating, timeSpan));

			return timer;
		}

		public virtual void Dispose()
		{
			TimerCallBack = null;
			StopTimer();
			gameObject.Destroy();
		}

		protected override void OnDestroy()
		{
			StopTimer();
			base.OnDestroy();
		}

		protected override void Awake()
		{
			base.Awake();
			
			Action act = OnTick;
			OnTickMethodName = act.Method.Name;
		}

		protected override void Start()
		{
			base.Start();

			if (!Autorun) return;
			StartTimer();
		}

		public void StartTimer()
		{
			if (IsRunning) return;
			var time = (float)Math.Min(TimeInterval.TotalSeconds, float.MaxValue);
			if (IsRepeating)
			{
				InvokeRepeating(OnTickMethodName, time, time);
			}
			else
			{
				Invoke(OnTickMethodName, time);
			}
			IsRunning = true;
		}

		public virtual void StopTimer()
		{
			CancelInvoke(OnTickMethodName);
			IsRunning = false;
		}

		private void OnTick()
		{
			//Debug.LogWarning(string.Format("<<< TICK ({0}, {1}, {2}) >>>", Autorun, IsRepeating, TimeInterval));

			if (TimerCallBack == null) return;
			TimerCallBack();
		}
	}
}