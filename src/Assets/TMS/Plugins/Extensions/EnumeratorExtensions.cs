#region Code Editor

// Maxim

#endregion

#region

using System.Collections;
using TMS.Common.Tasks.Threading;

#endregion

namespace TMS.Common.Extensions
{
	/// <summary>
	/// 
	/// </summary>
	public static class EnumeratorExtensions
	{
		/// <summary>
		///     Starts the Enumerator as async Task on the given TaskDistributor.
		/// </summary>
		/// <returns>The task.</returns>
		public static CustomTask RunAsync(this IEnumerator that)
		{
			return that.RunAsync(ThreadHelper.TaskDistributor);
		}

		/// <summary>
		/// Starts the Enumerator as async Task on the given TaskDistributor.
		/// </summary>
		/// <param name="that">The that.</param>
		/// <param name="target">The TaskDistributor instance on which the operation should perform.</param>
		/// <returns>
		/// The task.
		/// </returns>
		public static CustomTask RunAsync(this IEnumerator that, TaskDistributor target)
		{
			return target.Dispatch(CustomTask.Create(that));
		}
	}
}