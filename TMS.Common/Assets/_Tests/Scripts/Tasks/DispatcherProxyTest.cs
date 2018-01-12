using System;
using System.Threading;
using NUnit.Framework;
using TMS.Common.Core;
using TMS.Common.Tasks.Threading;
using UnityEngine;

namespace TMS.Common.Test.Tasks
{
	[TestFixture]
	public class DispatcherProxyTest
	{
		[Test]
		public void InvokeTest()
		{
			Debug.LogFormat("ManagedThreadId: {0}", Thread.CurrentThread.ManagedThreadId);

			var t1 = new Thread(OnNewThread);
			Assert.IsNotNull(t1);
			
			t1.Start();
		}

		private void OnNewThread()
		{
			Debug.LogFormat("OnNewThread: {0}", Thread.CurrentThread.ManagedThreadId);

			var t = ThreadHelper.Default.CurrentDispatcher.Dispatch(OnMainThread);

			Debug.LogFormat("Invoked Task Result: {0}", t);

			Assert.IsNotNull(t);
		}

		private void OnMainThread()
		{
			Debug.LogFormat("OnMainThread: {0}", Thread.CurrentThread.ManagedThreadId);
		}
	}
}