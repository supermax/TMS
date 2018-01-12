#region Code Editor

// Maxim

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	public enum TaskSortingSystem
	{
		/// <summary>
		///     The never reorder
		/// </summary>
		NeverReorder,

		/// <summary>
		///     The reorder when added
		/// </summary>
		ReorderWhenAdded,

		/// <summary>
		///     The reorder when executed
		/// </summary>
		ReorderWhenExecuted
	}
}