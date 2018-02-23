#region Code Editor

// Maxim

#endregion

#region

using System.Collections;
using System.Threading;
using TMS.Common.Extensions;

#endregion

namespace TMS.Common.Tasks.Threading
{
	/// <summary>
	/// 
	/// </summary>
	internal sealed class TaskWorker : CustomThread
	{
		public CustomTaskDispatcher Dispatcher;

		public TaskWorker(string name, TaskDistributor taskDistributor)
			: base(name, false)
		{
			TaskDistributor = taskDistributor;
			Dispatcher = new CustomTaskDispatcher(false);
		}

		public TaskDistributor TaskDistributor { get; private set; }

		public bool IsWorking
		{
			get { return Dispatcher.IsWorking; }
		}

		protected override IEnumerator Do()
		{
			while (!ExitEvent.InterWaitOne(0))
			{
				if (Dispatcher.ProcessNextTask()) continue;

				TaskDistributor.FillTasks(Dispatcher);

				if (Dispatcher.TaskCount != 0) continue;

				var result = WaitHandle.WaitAny(new[] {ExitEvent, TaskDistributor.NewDataWaitHandle});
				if (result == 0) return null;

				TaskDistributor.FillTasks(Dispatcher);
			}
			return null;
		}

		public override void Dispose()
		{
			base.Dispose();
			if (Dispatcher != null)
			{
				Dispatcher.Dispose();
			}
			Dispatcher = null;
		}
	}
}