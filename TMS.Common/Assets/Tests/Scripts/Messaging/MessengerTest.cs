#if UNITY_EDITOR && !UNITY_WSA
using NUnit.Framework;
#endif

using TMS.Common.Messaging;
using TMS.Common.Modularity;
using TMS.Common.Modularity.Ioc;

namespace TMS.Common.Test.Messaging
{
#if UNITY_EDITOR && !UNITY_WSA
    [TestFixture]
#endif
	public class MessengerTest
	{
#if UNITY_EDITOR && !UNITY_WSA
    [Test]
#endif
		public void PublishMsgTest()
		{
			IocManager.Default.Configure(GetType());

            IocManager.Default.Register<MyType>(new MyType { Name = "Maxim"});

			var instance = IocManager.Default.Resolve<ITestMessenger>();

            Messenger.Default.Publish(new TestMessengerPayload { Msg = "Hello Messenger!" });

			Messenger.Default.Publish(new TestMessengerPayload<int> { Data = 111 });

			Messenger.Default.Publish(new TestMessengerPayload1 { Data = new object() });

			Messenger.Default.Publish(new TestMessengerPayload<string> { Data = "1111" });

			Messenger.Default.Publish(new NewTestMessengerPayload { Data = 0.5 });

			Messenger.Default.Publish<ITestMessengerPayload>(new TestMessengerPayload { Msg = "interface payload test" });

			IBroadcastPayload args1 = new TestMessengerPayload { Msg = "interface payload test" };
			Messenger.Default.Publish(args1);
		}
	}
}