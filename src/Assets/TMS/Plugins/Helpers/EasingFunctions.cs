#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace TMS.Common.Helpers
{
	/// <summary>
	///     Easing Functions
	/// </summary>
	public static class EasingFunctions
	{
		/// <summary>
		///     no easing, no acceleration
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float LinearIn(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);
			return (changeValue*time/duration + startValue);
		}

		/// <summary>
		///     accelerating from zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float QuadraticIn(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration;
			return (changeValue*time*time + startValue);
		}

		/// <summary>
		///     decelerating to zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float QuadraticOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration;
			return (-changeValue*time*(time - 2) + startValue);
		}

		/// <summary>
		///     acceleration until halfway, then deceleration
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float QuadraticInOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration/2;
			if (time < 1) return (long) (changeValue*0.5f*time*time + startValue);
			time--;
			return (-changeValue*0.5f*(time*(time - 2) - 1) + startValue);
		}

		/// <summary>
		///     accelerating from zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float CubicIn(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration;
			return (changeValue*time*time*time + startValue);
		}

		/// <summary>
		///     decelerating to zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float CubicOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration;
			time--;
			return (changeValue*(time*time*time + 1) + startValue);
		}

		/// <summary>
		///     acceleration until halfway, then deceleration
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float CubicInOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration/2;
			if (time < 1) return (long) (changeValue*0.5f*time*time*time + startValue);
			time -= 2;
			return (changeValue*0.5f*(time*time*time + 2) + startValue);
		}

		/// <summary>
		///     accelerating from zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float QuarticIn(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration;
			return (changeValue*time*time*time*time + startValue);
		}

		/// <summary>
		///     decelerating to zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float QuarticOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration;
			time--;
			return (-changeValue*(time*time*time*time - 1) + startValue);
		}

		/// <summary>
		///     acceleration until halfway, then deceleration
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float QuarticInOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration/2;
			if (time < 1) return (long) (changeValue*0.5f*time*time*time*time + startValue);
			time -= 2;
			return (-changeValue*0.5f*(time*time*time*time - 2) + startValue);
		}

		/// <summary>
		///     accelerating from zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float QuinticIn(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration;
			return (changeValue*time*time*time*time*time + startValue);
		}

		/// <summary>
		///     decelerating to zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float QuinticOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration;
			time--;
			return (changeValue*(time*time*time*time*time + 1) + startValue);
		}

		/// <summary>
		///     acceleration until halfway, then deceleration
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float QuinticInOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration/2;
			if (time < 1) return (long) (changeValue*0.5f*time*time*time*time*time + startValue);
			time -= 2;
			return (changeValue*0.5f*(time*time*time*time*time + 2) + startValue);
		}

		/// <summary>
		///     accelerating from zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float SinusoidalIn(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			return (float) (-changeValue*Math.Cos(time/duration*(Math.PI*0.5f)) + changeValue + startValue);
		}

		/// <summary>
		///     decelerating to zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float SinusoidalOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			return (float) (changeValue*Math.Sin(time/duration*(Math.PI*0.5f)) + startValue);
		}

		/// <summary>
		///     accelerating until halfway, then decelerating
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float SinusoidalInOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			return (float) (-changeValue*0.5f*(Math.Cos(Math.PI*time/duration) - 1) + startValue);
		}

		/// <summary>
		///     accelerating from zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float ExponentialIn(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			return (float) (changeValue*Math.Pow(2, 10*(time/duration - 1)) + startValue);
		}

		/// <summary>
		///     decelerating to zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float ExponentialOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			return (float) (changeValue*(-Math.Pow(2, -10*time/duration) + 1) + startValue);
		}

		/// <summary>
		///     accelerating until halfway, then decelerating
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float ExponentialInOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration/2;
			if (time < 1) return (long) (changeValue*0.5f*Math.Pow(2, 10*(time - 1)) + startValue);
			time--;
			return (float) (changeValue*0.5f*(-Math.Pow(2, -10*time) + 2) + startValue);
		}

		/// <summary>
		///     accelerating from zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float CircularIn(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration;
			return (float) (-changeValue*(Math.Sqrt(1 - time*time) - 1) + startValue);
		}

		/// <summary>
		///     decelerating to zero velocity
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float CircularOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration*0.5f;
			if (time < 1) return (long) (-changeValue*0.5f*(Math.Sqrt(1 - time*time) - 1) + startValue);
			time -= 2;
			return (float) (changeValue*0.5f*(Math.Sqrt(1 - time*time) + 1) + startValue);
		}

		/// <summary>
		///     acceleration until halfway, then deceleration
		/// </summary>
		/// <param name="time">The time.</param>
		/// <param name="startValue">The start value.</param>
		/// <param name="endValue">The end value.</param>
		/// <param name="duration">The duration.</param>
		/// <returns></returns>
		private static float CircularInOut(float time, long startValue, long endValue, float duration)
		{
			var changeValue = (endValue - startValue);

			time /= duration/2;
			if (time < 1) return (long) (-changeValue*0.5f*(Math.Sqrt(1 - time*time) - 1) + startValue);
			time -= 2;
			return (float) (changeValue*0.5f*(Math.Sqrt(1 - time*time) + 1) + startValue);
		}

		/// <summary>
		///     The easing function by type
		/// </summary>
		public static readonly IDictionary<EasingFunctionType, Func<float, long, long, float, float>> EasingFuncByType =
			new Dictionary<EasingFunctionType, Func<float, long, long, float, float>>
			{
				{EasingFunctionType.Linear, LinearIn},
				{EasingFunctionType.QuadraticIn, QuadraticIn},
				{EasingFunctionType.QuadraticOut, QuadraticOut},
				{EasingFunctionType.QuadraticInOut, QuadraticInOut},
				{EasingFunctionType.CubicIn, CubicIn},
				{EasingFunctionType.CubicOut, CubicOut},
				{EasingFunctionType.CubicInOut, CubicInOut},
				{EasingFunctionType.QuarticIn, QuarticIn},
				{EasingFunctionType.QuarticOut, QuarticOut},
				{EasingFunctionType.QuarticInOut, QuarticInOut},
				{EasingFunctionType.QuinticIn, QuinticIn},
				{EasingFunctionType.QuinticOut, QuinticOut},
				{EasingFunctionType.QuinticInOut, QuinticInOut},
				{EasingFunctionType.SinusoidalIn, SinusoidalIn},
				{EasingFunctionType.SinusoidalOut, SinusoidalOut},
				{EasingFunctionType.SinusoidalInOut, SinusoidalInOut},
				{EasingFunctionType.ExponentialIn, ExponentialIn},
				{EasingFunctionType.ExponentialOut, ExponentialOut},
				{EasingFunctionType.ExponentialInOut, ExponentialInOut},
				{EasingFunctionType.CircularIn, CircularIn},
				{EasingFunctionType.CircularOut, CircularOut},
				{EasingFunctionType.CircularInOut, CircularInOut}
			};
	}

	/// <summary>
	/// </summary>
	public enum EasingFunctionType
	{
		/// <summary>
		///     The linear
		/// </summary>
		Linear,

		/// <summary>
		///     The quadratic in
		/// </summary>
		QuadraticIn,

		/// <summary>
		///     The quadratic out
		/// </summary>
		QuadraticOut,

		/// <summary>
		///     The quadratic in out
		/// </summary>
		QuadraticInOut,

		/// <summary>
		///     The cubic in
		/// </summary>
		CubicIn,

		/// <summary>
		///     The cubic out
		/// </summary>
		CubicOut,

		/// <summary>
		///     The cubic in out
		/// </summary>
		CubicInOut,

		/// <summary>
		///     The quartic in
		/// </summary>
		QuarticIn,

		/// <summary>
		///     The quartic out
		/// </summary>
		QuarticOut,

		/// <summary>
		///     The quartic in out
		/// </summary>
		QuarticInOut,

		/// <summary>
		///     The quintic in
		/// </summary>
		QuinticIn,

		/// <summary>
		///     The quintic out
		/// </summary>
		QuinticOut,

		/// <summary>
		///     The quintic in out
		/// </summary>
		QuinticInOut,

		/// <summary>
		///     The sinusoidal in
		/// </summary>
		SinusoidalIn,

		/// <summary>
		///     The sinusoidal out
		/// </summary>
		SinusoidalOut,

		/// <summary>
		///     The sinusoidal in out
		/// </summary>
		SinusoidalInOut,

		/// <summary>
		///     The exponential in
		/// </summary>
		ExponentialIn,

		/// <summary>
		///     The exponential out
		/// </summary>
		ExponentialOut,

		/// <summary>
		///     The exponential in out
		/// </summary>
		ExponentialInOut,

		/// <summary>
		///     The circular in
		/// </summary>
		CircularIn,

		/// <summary>
		///     The circular out
		/// </summary>
		CircularOut,

		/// <summary>
		///     The circular in out
		/// </summary>
		CircularInOut
	}
}