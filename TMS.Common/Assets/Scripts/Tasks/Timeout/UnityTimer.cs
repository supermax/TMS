#define UNITY3D

#region Usings

using System;
using TMS.Common.Core;
using TMS.Common.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

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
			ArgumentValidator.AssertNotEquals(timeSpan, TimeSpan.Zero, "timeSpan", "timeSpan cannot be {0}", TimeSpan.Zero);
			
			const string timersHostName = "[TIMERS]";

			var go = GameObject.Find(timersHostName);
			if (go == null)
			{
				go = new GameObject(timersHostName)
							{
								hideFlags = HideFlags.DontSave
							};
				Debug.LogFormat("Created {0}", go.name);
				if (Application.isPlaying)
				{
					DontDestroyOnLoad(go);
				}
			}
			
			var timer = go.AddComponent<UnityTimer>();
			timer.name = string.Format("Timer_{0}_{1}", timerCallback, timerCallback.GetHashCode());

			timer.TimeInterval = timeSpan;
			timer.IsRepeating = isRepeating;
			timer.TimerCallBack = timerCallback;
			timer.Autorun = autorun;
			Debug.LogFormat("Created new timer (name: {0}, autorun: {1}, repeating: {2}, interval: {3})", timer.name, autorun, isRepeating, timeSpan);

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
#if !UNITY_WSA
			OnTickMethodName = act.Method.Name;
#else
			OnTickMethodName = act.ToString();
#endif
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