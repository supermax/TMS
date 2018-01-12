using TMS.Common.Messaging;

namespace TMS.Common.Test.Messaging
{
	public interface ITestMessengerPayload : IBroadcastPayload
	{
		string Msg { get; set; }
	}

	public class TestMessengerPayload : ITestMessengerPayload
	{
		public string Msg { get; set; }

		public IBroadcastPayloadInfo Info
		{
			get;
			set;
		}
		
		public bool IsTraceEnabled
		{
			get;
			set;
		}

		public bool IsHandled
		{
			get;
			set;
		}
	}

	public class TestMessengerPayload1 : TestMessengerPayload
	{
		public object Data { get; set; }
	}

	public class TestMessengerPayload<T> : TestMessengerPayload
	{
		public T Data { get; set; }
	}

	public class NewTestMessengerPayload : TestMessengerPayload<double>
	{
		
	}
}