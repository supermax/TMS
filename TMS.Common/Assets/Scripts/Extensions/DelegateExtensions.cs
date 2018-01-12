#define UNITY3D

#region Usings

using System;
using System.ComponentModel;
using System.Reflection;
using TMS.Common.Core;
using TMS.Common.Tasks.Threading;

#endregion

namespace TMS.Common.Extensions
{
	/// <summary>
	///     Delegate Extensions
	/// </summary>
	public static class DelegateExtensions
	{
		/// <summary>
		///     Creates the delegate unique id.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public static string CreateDelegateUniqueId(this Delegate method)
		{
			MethodInfo methodInfo = null;
#if !NETFX_CORE
			methodInfo = method.Method;
#else
			methodInfo = method.GetMethodInfo();
#endif
			var id = string.Format("{0} ({1}_{2})", 
										methodInfo, 
										method.Target != null ? method.Target.GetHashCode() : 0, 
										method.GetHashCode());
			return id;
		}

		/// <summary>
		///     Safes the invoke.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="method">The method.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public static bool SafeInvoke(this object sender, Delegate method, params object[] args)
		{
			if (method == null) return false;
			Func<object> func = () => method.DynamicInvoke(args);
			ThreadHelper.Default.CurrentDispatcher.Dispatch(func);
            return true;
		}

		/// <summary>
		///     Safely invokes event handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="handler">The handler.</param>
		/// <param name="args">
		///     The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		/// <returns>
		///     <c>true</c> if event handler invoked
		/// </returns>
		public static bool SafeInvoke(this object sender, EventHandler handler, EventArgs args)
		{
			if (handler == null) return false;
			Action act = () => handler(sender, args);
			ThreadHelper.Default.CurrentDispatcher.Dispatch(act);
            return true;
		}

		/// <summary>
		///     Safely invokes event handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="handler">The handler.</param>
		/// <param name="args">
		///     The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		/// <returns>
		///     <c>true</c> if event handler invoked
		/// </returns>
		public static bool SafeInvoke<T>(this object sender, EventHandler<T> handler, T args) where T : EventArgs
		{
			if (handler == null) return false;
			Action act = () => handler(sender, args);
			ThreadHelper.Default.CurrentDispatcher.Dispatch(act);
            return true;
		}

		/// <summary>
		///     Safely invokes event handler.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">
		///     The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		/// <returns>
		///     <c>true</c> if event handler invoked
		/// </returns>
		public static bool SafeInvoke(this Delegate method, params object[] args)
		{
			if (method == null) return false;
			Func<object> func = () => method.DynamicInvoke(args);
			ThreadHelper.Default.CurrentDispatcher.Dispatch(func);
            return true;
		}

		/// <summary>
		///     Safely invokes event handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="handler">The handler.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		///     <c>true</c> if event handler invoked
		/// </returns>
		public static bool SafeInvoke(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler,
		                              string propertyName)
		{
			return SafeInvoke(sender, handler, new PropertyChangedEventArgs(propertyName));
		}

		/// <summary>
		///     Safely invokes event handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="handler">The handler.</param>
		/// <param name="args">
		///     The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		/// <returns>
		///     <c>true</c> if event handler invoked
		/// </returns>
		public static bool SafeInvoke(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler, PropertyChangedEventArgs args)
		{
			if (handler == null) return false;
			Action act = () => handler(sender, args);
			ThreadHelper.Default.CurrentDispatcher.Dispatch(act);
            return true;
		}
	}
}