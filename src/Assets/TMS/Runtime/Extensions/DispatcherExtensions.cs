#region Usings

using System;
using TMS.Common.Core;

#endregion

namespace TMS.Common.Extensions
{
	/// <summary>
	///     Dispatcher Extensions
	/// </summary>
	public static class DispatcherExtensions
	{
#if !NETFX_CORE
		/// <summary>
		///     Safely invokes the specified method.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		public static void SafeBeginInvoke(this DispatcherProxy sender, Delegate method, params object[] args)
		{
			if (sender == null || sender.CheckAccess())
			{
				method.DynamicInvoke(args);
				return;
			}
			sender.BeginInvoke(method, args);
		}

		/// <summary>
		///     Safely invokes the specified method.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public static object SafeInvoke(this DispatcherProxy sender, Delegate method, params object[] args)
		{
			if (sender == null || sender.CheckAccess())
			{
				return method.DynamicInvoke(args);
			}

#if !SILVERLIGHT
			return sender.Invoke(method, args);
#else
			return sender.BeginInvoke(method, args);
#endif
		}

		/// <summary>
		///     Begins the invocation of the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		public static void BeginInvoke(Delegate method, params object[] args)
		{
			if (method == null) return;
			var disp = DispatcherProxy.Default;
			if (disp != null)
			{
				disp.BeginInvoke(method,
#if !SILVERLIGHT
								DispatcherPriority.Normal,
#endif
								args);
			}
			else
			{
				method.DynamicInvoke(args);
			}
		}

		/// <summary>
		///     Invokes the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public static object Invoke(Delegate method, params object[] args)
		{
			if (method == null) return null;
			var disp = DispatcherProxy.Default;
			if (disp != null)
			{
#if !SILVERLIGHT
				return disp.Invoke(method, DispatcherPriority.Normal, args);
#else
				return disp.BeginInvoke(method, args);
#endif
			}

			return method.DynamicInvoke(args);
		}
#else
		/// <summary>
		///     Safely invokes the specified method.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		public static void SafeBeginInvoke(this CoreDispatcher sender, Delegate method, params object[] args)
		{
			if (sender == null || sender.HasThreadAccess)
			{
				method.DynamicInvoke(args);
				return;
			}
			sender.RunAsync(CoreDispatcherPriority.Normal, () => method.DynamicInvoke(args));
		}

		/// <summary>
		///     Safely invokes the specified method.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public static async Task<object> SafeInvoke(this CoreDispatcher sender, Delegate method, params object[] args)
		{
			if (sender == null || sender.HasThreadAccess)
			{
				return method.DynamicInvoke(args);
			}

			object res = null;
			await sender.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				res = method.DynamicInvoke(args);
			});
			return res;
		}

		/// <summary>
		///     Begins the invocation of the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		public static void BeginInvoke(Delegate method, params object[] args)
		{
			if (method == null) return;
			var disp = DispatcherProxy.CreateDispatcher();
			if (disp != null)
			{
				disp.BeginInvoke(method, CoreDispatcherPriority.Normal, args);
			}
			else
			{
				method.DynamicInvoke(args);
			}
		}

		/// <summary>
		///     Invokes the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public static object Invoke(Delegate method, params object[] args)
		{
			if (method == null) return null;
			var disp = DispatcherProxy.CreateDispatcher();
			if (disp != null)
			{
				return disp.Invoke(method, CoreDispatcherPriority.Normal, args);
			}

			return method.DynamicInvoke(args);
		}
#endif
	}
}