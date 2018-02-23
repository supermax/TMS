using System.Threading;

namespace TMS.Common.Tasks.Threading
{
	public static class ThreadingExtensions
	{
		public static void Close(this ManualResetEvent e)
		{
#if UNITY_WSA && !UNITY_EDITOR
			e.Dispose();
#else
			e.Close();
#endif
		}
	}
}