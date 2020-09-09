using TMS.Common.Core;
using TMS.Common.Messaging;
using UnityEngine;

namespace TMS.Common.Test.Messaging
{
	//public interface IGameManager
	//{
	//    INetworkManager NetMan { get; }

	//    ICacheBroker CacheMan { get; }
	//}


	public class MyType
	{
		public string Name { get; set; }

		public MyType()
		{
			Name = "...";
		}
	}

	[MessengerConsumer(typeof(ITestMessenger), 
		IsSingleton = true, 
		InstantiateOnRegistration = true)]
	public class TestMessenger : Singleton<ITestMessenger, TestMessenger>, ITestMessenger
	{
		public TestMessenger(MyType obj)
		{
			Debug.LogFormat("{0}->CTOR, obj.Name: {1}", typeof(TestMessenger), obj.Name);
		}

		public TestMessenger()
		{
			Debug.LogFormat("{0}->CTOR", typeof(TestMessenger));
		}

		public void Subscribe()
		{
			Messenger.Default.Subscribe<TestMessengerPayload>(OnTestMessenger);


			Messenger.Default.Subscribe<TestMessengerPayload1>(OnTestMessenger);

			Messenger.Default.Subscribe<TestMessengerPayload<object>>(OnTestMessenger);
//
			Messenger.Default.Subscribe<TestMessengerPayload<int>>(OnTestMessenger);

			Messenger.Default.Subscribe<TestMessengerPayload<string>>(OnTestMessenger);

			Messenger.Default.Subscribe<NewTestMessengerPayload>(OnTestMessenger, payload => payload.Msg == "OK", true);

			Messenger.Default.Subscribe<IBroadcastPayload>(OnTestBroadcast);

			Messenger.Default.Subscribe<ITestMessengerPayload>(OnITestMessengerPayload);
		}

		private void OnITestMessengerPayload(ITestMessengerPayload payload)
		{
			//Messenger.Default.Publish<IBroadcastPayload>(payload);
		}

		private void OnTestBroadcast(IBroadcastPayload args)
		{
			Debug.LogFormat("IBroadcastPayload (MSG: \"{0}\")", args);
		}

		private void OnTestMessenger(NewTestMessengerPayload args)
		{
			Debug.LogFormat("OnTestMessenger (MSG: \"{0}\")", args.Msg);
		}

		private void OnTestMessenger(TestMessengerPayload args)
		{
			Debug.LogFormat("OnTestMessenger (MSG: \"{0}\")", args.Msg);
		}

		private void OnTestMessenger(TestMessengerPayload<string> args)
		{
			Debug.LogFormat("OnTestMessenger (MSG: \"{0}\")", args.Msg);
		}

	}
}