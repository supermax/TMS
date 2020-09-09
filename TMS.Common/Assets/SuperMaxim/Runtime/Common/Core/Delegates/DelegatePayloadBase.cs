#region

using System;

#endregion

namespace TMS.Common.Core
{
	/// <summary>
	/// Delegate Payload Base
	/// </summary>
	public abstract class DelegatePayloadBase : IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DelegatePayloadBase"/> class.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="methodArgs">The method args.</param>
		protected DelegatePayloadBase(Delegate method = null, params object[] methodArgs)
		{
			//ArgumentValidator.AssertNotNull(method, "method");

			if (method != null)
			{
				Method = new DelegateReference(method);
			}
			MethodArgs = methodArgs;
		}

		/// <summary>
		/// Gets the method.
		/// </summary>
		/// <value>
		/// The method.
		/// </value>
		internal DelegateReference Method { get; private set; }

		/// <summary>
		/// Gets the method args.
		/// </summary>
		/// <value>
		/// The method args.
		/// </value>
		internal object[] MethodArgs { get; private set; }

		/// <summary>
		/// Invokes this instance.
		/// </summary>
		internal void Invoke()
		{
			if (Method == null) return;
			if (MethodArgs != null)
			{
				Method.Invoke(MethodArgs);
			}
			else
			{
				Method.Invoke();
			}
		}

		protected internal bool IsDisposed;

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public virtual void Dispose()
		{
			if (IsDisposed) return;
			IsDisposed = true;

			if (Method != null)
			{
				Method.Dispose();
				Method = null;
			}
			MethodArgs = null;
		}
	}
}