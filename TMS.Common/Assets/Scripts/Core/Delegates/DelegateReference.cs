#define UNITY3D

#region Usings

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using TMS.Common.Extensions;
using TMS.Common.Tasks.Threading;
#if NETFX_CORE
using Windows.UI.Core;
#else
#if !UNITY3D
using System.Windows.Threading;
using System.Threading.Tasks;
#endif
#endif

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	/// Represents a reference to a <see cref="System.Delegate" /> that may contain a
	/// <see cref="WeakReference" /> to the target. This class is used
	/// internally by the Composite Application Library.
	/// </summary>
	[ComVisible(true)]
	internal sealed class DelegateReference : IWeakDelegate
	{
		private Delegate _delegate;
		private MethodInfo _method;

		private WeakReference _wr;

		public object Target
		{
			[SecuritySafeCritical]
			get { return _delegate != null ? _delegate.Target :  (_wr != null ? _wr.Target : null); }
		}
		
		/// <summary>
		/// Gets a value indicating whether this instance is alive.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
		/// </value>
		public bool IsAlive
		{
			[SecuritySafeCritical]
			get { return _delegate != null || (_wr.IsAlive && _wr.Target != null); }
		}
		
		/// <summary>
		///     Initializes a new instance of <see cref="DelegateReference" />.
		/// </summary>
		/// <param name="delegate">
		///     The original <see cref="System.Delegate" /> to create a reference for.
		/// </param>
		/// <param name="keepReferenceAlive">
		///     If <see langword="false" /> the class will create a weak reference to the delegate, allowing it to be garbage collected. Otherwise it will keep a strong reference to the target.
		/// </param>
		/// <param name="longRef">
		///     if set to <c>true</c> [long ref].
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     If the passed <paramref name="delegate" /> is not assignable to <see cref="System.Delegate" />.
		/// </exception>
		[SecuritySafeCritical]
		public DelegateReference(Delegate @delegate, bool keepReferenceAlive = false,
			bool longRef = false)
		{
			ArgumentValidator.AssertNotNull(@delegate, "delegate");

			Id = @delegate.CreateDelegateUniqueId();
#if !NETFX_CORE
			var methodInfo = @delegate.Method;
#else
			var methodInfo = @delegate.GetMethodInfo();
#endif
			if (keepReferenceAlive || (methodInfo.IsStatic || @delegate.Target == null))
			{
				_delegate = @delegate;
			}
			else
			{
				_method = methodInfo;
			}
			_wr = new WeakReference(@delegate.Target, longRef);
		}

		/// <summary>
		///     Gets the id.
		/// </summary>
		public string Id { get; private set; }

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_delegate = null;
			_method = null;
			_wr.Target = null;
			_wr = null;
		}

		/// <summary>
		/// Invokes the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public object Invoke(params object[] args)
		{
			try
			{
				object res;
				if (_delegate != null)
				{
					res = _delegate.DynamicInvoke(args);
				}
				else
				{
					res = _method.Invoke(_wr.Target, args);
				}

				return res;
			}
			catch (Exception e)
			{
#if UNITY || UNITY3D || UNITY_3D
				UnityEngine.Debug.LogException(e);
#else
				Console.WriteLine(e);
#endif
				throw;
			}
		}

		/// <summary>
		/// Begins the invoke.
		/// </summary>
		/// <param name="args">The args.</param>
		public void BeginInvoke(params object[] args)
		{
			try
			{
				Invoke(args);
			}
			catch (Exception e)
			{
#if UNITY || UNITY3D || UNITY_3D
				UnityEngine.Debug.LogException(e);
#else
				Console.WriteLine(e);
#endif
				throw;
			}
		}
	}
}
