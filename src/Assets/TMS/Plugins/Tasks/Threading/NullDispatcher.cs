#region Code Editor

// Maxim

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	public class NullDispatcher : CustomTaskDispatcherBase
	{
		/// <summary>
		///     The null
		/// </summary>
		public static NullDispatcher Null = new NullDispatcher();

		/// <summary>
		///     Checks the access limitation.
		/// </summary>
		protected override void CheckAccessLimitation()
		{
		}

		/// <summary>
		///     Adds the task.
		/// </summary>
		/// <param name="task">The task.</param>
		internal override void AddTask(CustomTask task)
		{
			task.DoInternal();
		}
	}
}