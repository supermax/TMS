#region Code Editor

// Maxim

#endregion

#region

using System;
using System.Collections;

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class EnumeratableActionThread : CustomThread
	{
		private readonly Func<CustomThread, IEnumerator> _enumeratableAction;

		/// <summary>
		/// Creates a new Thread which runs the given enumerable action.
		/// The thread will start running after creation.
		/// </summary>
		/// <param name="enumeratableAction">The enumerable action.</param>
		public EnumeratableActionThread(Func<CustomThread, IEnumerator> enumeratableAction)
			: this(enumeratableAction, true)
		{
		}

		/// <summary>
		/// Creates a new Thread which runs the given enumerable action.
		/// </summary>
		/// <param name="enumeratableAction">The enumerable action.</param>
		/// <param name="autoStartThread">Should the thread start after creation.</param>
		public EnumeratableActionThread(Func<CustomThread, IEnumerator> enumeratableAction, bool autoStartThread)
			: base("EnumeratableActionThread", CustomTaskDispatcher.Current, false)
		{
			_enumeratableAction = enumeratableAction;

			if (autoStartThread)
			{
				Start();
			}
		}

		/// <summary>
		/// Does this instance.
		/// </summary>
		/// <returns></returns>
		protected override IEnumerator Do()
		{
			return _enumeratableAction(this);
		}
	}
}