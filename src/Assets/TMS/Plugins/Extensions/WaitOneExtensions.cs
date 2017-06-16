#region Code Editor

// Maxim

#endregion

#region

using System;
using System.Threading;

#endregion

namespace TMS.Common.Extensions
{
	/// <summary>
	/// </summary>
	public static class WaitOneExtensions
	{
#if UNITY3D
		private static readonly System.Reflection.MethodInfo WaitOneMilliseconds;
		private static readonly System.Reflection.MethodInfo WaitOneTimeSpan;

		static WaitOneExtensions()
		{
			var type = typeof(ManualResetEvent);
			WaitOneMilliseconds = type.GetMethod("WaitOne", new [] { typeof(int) });
			WaitOneTimeSpan = type.GetMethod("WaitOne", new [] { typeof(TimeSpan) });
		}

		public static bool InterWaitOne(this ManualResetEvent that, int ms)
		{
			return (bool)WaitOneMilliseconds.Invoke(that, new object[] { ms });
		}

		public static bool InterWaitOne(this ManualResetEvent that, TimeSpan duration)
		{
			return (bool)WaitOneTimeSpan.Invoke(that, new object[] { duration });
		}
#else
		/// <summary>
		///     Inters the wait one.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="ms">The ms.</param>
		/// <returns></returns>
		public static bool InterWaitOne(this ManualResetEvent that, int ms)
		{
			return that.WaitOne(ms, false);
		}

		/// <summary>
		///     Inters the wait one.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		public static bool InterWaitOne(this ManualResetEvent that, TimeSpan duration)
		{
			return that.WaitOne(duration, false);
		}
#endif
	}
}