#region

using System;

#endregion

#if !UNITY3D
using System.Threading.Tasks;
using System.Windows.Threading;
#endif

namespace TMS.Common.Core
{
	public interface IDispatcherProxy
	{
		/// <summary>
		///     Checks is current code runs on dispatchers thread
		/// </summary>
		/// <returns></returns>
		bool CheckAccess();

		/// <summary>
		///     Begins the invoke.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		DispatcherOperation BeginInvoke(Delegate method,
#if !SILVERLIGHT
		                                DispatcherPriority priority = DispatcherPriority.Normal,
#endif
		                                params object[] args);

		/// <summary>
		///     Invokes the specified method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="priority">The priority.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
		Task<object> Invoke(Delegate method,
#if !SILVERLIGHT
		                    DispatcherPriority priority = DispatcherPriority.Normal,
#endif
		                    params object[] args);
	}
}