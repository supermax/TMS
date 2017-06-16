#region

using System;
using System.Runtime.Remoting.Messaging;
using TMS.Common.Tasks.Threading;

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	///     Hides the dispatcher mis-match between Silverlight and .Net, largely so code reads a bit easier
	/// </summary>
	public class DispatcherProxy : Singleton<IDispatcherProxy, DispatcherProxy>, IDispatcherProxy
	{
		private readonly CustomTaskDispatcher _dispatcher;

		/// <summary>
		/// Initializes a new instance of the <see cref="DispatcherProxy"/> class.
		/// </summary>
		public DispatcherProxy()
		{
			_dispatcher = ThreadHelper.Dispatcher;
		}

		/// <summary>
		///     Checks is current code runs on dispatchers thread
		/// </summary>
		/// <returns></returns>
		public bool CheckAccess()
		{
			return ThreadHelper.Default.IsMainThread();
		}

		/// <summary>
		/// Invokes the internal.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		protected virtual DispatcherOperation InvokeInternal(Delegate method, DispatcherPriority priority, params object[] args)
		{
			var dispOperation = new DispatcherOperation { Method = method };
			Action act = () => dispOperation.Invoke(args);
			if (CheckAccess())
			{
				act();
			}
			else
			{
				if (_dispatcher == null)
				{
#if UNITY3D					
					UnityEngine.Debug.LogError("'_dispatcher' is null");
#else
					System.Diagnostics.Debug.WriteLine("'_dispatcher' is null");
#endif
					return null;
				}
				
				_dispatcher.Dispatch(act);
			}
			return dispOperation;
		}

		/// <summary>
		///     Begins the invoke.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public DispatcherOperation BeginInvoke(Delegate method, DispatcherPriority priority, params object[] args)
		{
		   return InvokeInternal(method, priority, args); // TODO should be async
		}

		/// <summary>
		///     Invokes the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public Task<object> Invoke(Delegate method, DispatcherPriority priority, params object[] args)
		{
			return InvokeInternal(method, priority, args);
		}

		/// <summary>
		/// Begins the invoke.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public DispatcherOperation BeginInvoke(Delegate method, params object[] args)
		{
			return InvokeInternal(method, DispatcherPriority.Normal, args);
		}

		/// <summary>
		/// Invokes the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public Task<object> Invoke(Delegate method, params object[] args)
		{
			return InvokeInternal(method, DispatcherPriority.Normal, args);
		}
	}

#if UNITY3D
	/// <summary>
	/// 
	/// </summary>
	public enum DispatcherPriority
	{
		/// <summary>
		/// The normal
		/// </summary>
		Normal
	}

	/// <summary>
	/// Dispatcher Operation
	/// </summary>
	/// <seealso cref="object" />
	public class DispatcherOperation : Task<object>
	{
		
	}

	/// <summary>
	/// 
	/// </summary>
	/// <seealso cref="object" />
	public class Task : Task<object>
	{

	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <seealso cref="object" />
	public class Task<T>
	{
		/// <summary>
		/// Gets or sets the result.
		/// </summary>
		/// <value>
		/// The result.
		/// </value>
		public T Result { get; protected internal set; }

		/// <summary>
		/// Gets or sets the method.
		/// </summary>
		/// <value>
		/// The method.
		/// </value>
		public Delegate Method { get; protected internal set; }

		/// <summary>
		/// Invokes the method with specified arguments.
		/// </summary>
		/// <param name="args">The arguments.</param>
		/// <returns></returns>
		public virtual T Invoke(params object[] args)
		{
			Result = (T)Method.DynamicInvoke(args);
			return Result;
		}
	}
#endif
}