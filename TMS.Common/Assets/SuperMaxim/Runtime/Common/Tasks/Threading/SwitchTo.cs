#region Code Editor

// Maxim

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// </summary>
	public class SwitchTo
	{
		/// <summary>
		/// </summary>
		public enum TargetType
		{
			/// <summary>
			///     The main
			/// </summary>
			Main,

			/// <summary>
			///     The thread
			/// </summary>
			Thread
		}

		/// <summary>
		///     Changes the context of the following commands to the MainThread when yielded.
		/// </summary>
		public static readonly SwitchTo MainThread = new SwitchTo(TargetType.Main);

		/// <summary>
		///     Changes the context of the following commands to the WorkerThread when yielded.
		/// </summary>
		public static readonly SwitchTo Thread = new SwitchTo(TargetType.Thread);

		private SwitchTo(TargetType target)
		{
			Target = target;
		}

		/// <summary>
		///     Gets the target.
		/// </summary>
		/// <value>
		///     The target.
		/// </value>
		public TargetType Target { get; private set; }
	}
}