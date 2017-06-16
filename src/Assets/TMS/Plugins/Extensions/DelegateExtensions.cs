#region

using System;
using System.ComponentModel;
using System.Reflection;
using TMS.Common.Core;

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
			var id = String.Format("{0} ({1}_{2})", methodInfo, method.Target != null ? method.Target.GetHashCode() : 0,
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
            DispatcherProxy.Default.Invoke(method, DispatcherPriority.Normal, args);
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
            DispatcherProxy.Default.Invoke(handler, DispatcherPriority.Normal, sender, args);
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
            DispatcherProxy.Default.Invoke(handler, DispatcherPriority.Normal, sender, args);
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
            DispatcherProxy.Default.Invoke(method, DispatcherPriority.Normal, args);
            return true;
		}

#if !NETFX_CORE
		/// <summary>
		///     Safely invokes event handler.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="method">The method.</param>
		/// <param name="result">The result.</param>
		/// <param name="args">
		///     The <see cref="System.EventArgs" /> instance containing the event data.
		/// </param>
		/// <returns>
		///     <c>true</c> if event handler invoked
		/// </returns>
		public static Task<T> SafeInvoke<T>(this Delegate method, params object[] args)
		{
			if (method == null) return null;

			var result = new Task<T>();
            var res = DispatcherProxy.Default.Invoke(method,
#if !SILVERLIGHT
                                      DispatcherPriority.Normal,
#endif
                                      args);
            result.Result = (T)res.Result;
            return result;
		}
#endif

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
		public static bool SafeInvoke(this INotifyPropertyChanged sender, PropertyChangedEventHandler handler,
		                              PropertyChangedEventArgs args)
		{
			if (handler == null) return false;
            DispatcherProxy.Default.Invoke(handler, DispatcherPriority.Normal, sender, args);
            return true;
		}

#if !NETFX_CORE && !UNITY3D
		/// <summary>
		///     Safely invokes event handler.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="handler">The handler.</param>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		///     <c>true</c> if event handler invoked
		/// </returns>
		public static bool SafeInvoke(this INotifyPropertyChanging sender, PropertyChangingEventHandler handler,
		                              string propertyName)
		{
			var res = SafeInvoke(sender, handler, new PropertyChangingEventArgs(propertyName));
			return res;
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
		public static bool SafeInvoke(this INotifyPropertyChanging sender, PropertyChangingEventHandler handler,
		                              PropertyChangingEventArgs args)
		{
			if (handler == null) return false;
			var disp = DispatcherProxy.CreateDispatcher();
			if (disp == null)
			{
				handler(sender, args);
			}
			else
			{
				disp.Invoke(handler,
#if !SILVERLIGHT
				            DispatcherPriority.Normal,
#endif
				            sender, args);
			}
			return true;
		}
#endif
	}
}