#region

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using TMS.Common.Extensions;

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
	public class DelegateReference : IDisposable
	{
		protected object Target; // NOTE: do not remove! holds ref to object in case of keepReferenceAlive = true
		protected Delegate Delegate;
		protected MethodInfo Method;

		private readonly WeakReference _wr;

		protected internal WeakReference WeakReference
		{
			get { return _wr; }
		}

//		/// <summary>
//		/// Initializes a new instance of the <see cref="DelegateReference"/> class.
//		/// </summary>
//		/// <param name="target">The target.</param>
//		/// <param name="keepReferenceAlive">if set to <c>true</c> [keep reference alive].</param>
//		/// <param name="dispatcherPriority">The dispatcher priority.</param>
//		/// <param name="longRef">if set to <c>true</c> [long ref].</param>
//		[SecuritySafeCritical]
//		public DelegateReference(object target, bool keepReferenceAlive = false,
//#if !SILVERLIGHT
//#if !NETFX_CORE
//								 DispatcherPriority dispatcherPriority = DispatcherPriority.Normal,
//#else
// CoreDispatcherPriority dispatcherPriority = CoreDispatcherPriority.Normal,
//#endif
//#endif
// bool longRef = false)
//		{
//			ArgumentValidator.AssertNotNull(target, "target");

//			Id = target.GetHashCode().ToString(CultureInfo.InvariantCulture);
//#if !SILVERLIGHT
//			DispatcherPriority = dispatcherPriority;
//#endif
//			if (keepReferenceAlive)
//			{
//				Target = target;
//			}
//			_wr = new WeakReference(target, longRef);
//		}

		/// <summary>
		///     Initializes a new instance of <see cref="DelegateReference" />.
		/// </summary>
		/// <param name="delegate">
		///     The original <see cref="System.Delegate" /> to create a reference for.
		/// </param>
		/// <param name="keepReferenceAlive">
		///     If <see langword="false" /> the class will create a weak reference to the delegate, allowing it to be garbage collected. Otherwise it will keep a strong reference to the target.
		/// </param>
		/// <param name="dispatcherPriority">The dispatcher priority.</param>
		/// <param name="longRef">
		///     if set to <c>true</c> [long ref].
		/// </param>
		/// <exception cref="ArgumentNullException">
		///     If the passed <paramref name="delegate" /> is not assignable to <see cref="System.Delegate" />.
		/// </exception>
		[SecuritySafeCritical]
		public DelegateReference(Delegate @delegate, bool keepReferenceAlive = false,
#if !SILVERLIGHT
#if !NETFX_CORE
								 DispatcherPriority dispatcherPriority = DispatcherPriority.Normal,
#else
 CoreDispatcherPriority dispatcherPriority = CoreDispatcherPriority.Normal,
#endif
#endif
 bool longRef = false)
		{
			ArgumentValidator.AssertNotNull(@delegate, "delegate");

			Id = @delegate.CreateDelegateUniqueId();
#if !SILVERLIGHT
			DispatcherPriority = dispatcherPriority;
#endif
#if !NETFX_CORE
			var methodInfo = @delegate.Method;
#else
			var methodInfo = @delegate.GetMethodInfo();
#endif
			if (keepReferenceAlive || (methodInfo.IsStatic || @delegate.Target == null))
			{
				Delegate = @delegate;
			}
			else
			{
				Method = methodInfo;
			}
			_wr = new WeakReference(@delegate.Target, longRef);
		}

#if !SILVERLIGHT
#if !NETFX_CORE
		/// <summary>
		///     Gets the dispatcher priority.
		/// </summary>
		internal protected DispatcherPriority DispatcherPriority { get; private set; }
#else
		/// <summary>
		///     Gets the dispatcher priority.
		/// </summary>
		internal protected CoreDispatcherPriority DispatcherPriority { get; private set; }
#endif
#endif

		/// <summary>
		///     Gets the id.
		/// </summary>
		public virtual string Id { get; private set; }

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			Delegate = null;
			Method = null;
			Target = null;
			_wr.Target = null;
		}

		/// <summary>
		/// Invokes the specified args.
		/// </summary>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		public virtual object Invoke(params object[] args)
		{
			Task<object> res;
			if (Delegate != null)
			{
				res = DispatcherProxy.Default.Invoke(Delegate,
#if !SILVERLIGHT
 DispatcherPriority,
#endif
 args);
			}
			else
			{
				Func<object> act = () => Method.Invoke(_wr.Target, args);
				res = DispatcherProxy.Default.Invoke(act
#if !SILVERLIGHT
, DispatcherPriority
#endif
);
			}
			return res.Result;
		}

		/// <summary>
		/// Begins the invoke.
		/// </summary>
		/// <param name="args">The args.</param>
		public virtual void BeginInvoke(params object[] args)
		{
			if (Delegate != null)
			{
				DispatcherProxy.Default.BeginInvoke(Delegate,
#if !SILVERLIGHT
 DispatcherPriority,
#endif
 args);
			}
			else
			{
				Func<object> act = () => Method.Invoke(_wr.Target, args);
				DispatcherProxy.Default.BeginInvoke(act
#if !SILVERLIGHT
, DispatcherPriority
#endif
);
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is alive.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
		/// </value>
		public virtual bool IsAlive
		{
			[SecuritySafeCritical]
			get { return Delegate != null || (_wr.IsAlive && _wr.Target != null); }
		}
	}
}
