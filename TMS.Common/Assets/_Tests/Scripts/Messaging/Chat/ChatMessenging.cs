#if UNITY_EDITOR && !UNITY_WSA
using NUnit.Framework;
#endif

using TMS.Common.Messaging;

namespace TMS.Common.Test.Messaging
{
#if UNITY_EDITOR && !UNITY_WSA
	[TestFixture]
#endif
	public class ChatMessenging
	{
#if UNITY_EDITOR && !UNITY_WSA
		[Test]
#endif
		public void Subscribe()
		{
			Messenger.Default.Subscribe<ChatLoginPayload>(OnChatLogin, FilterMsg);

			Publish();
		}

		public void Publish()
		{
			Messenger.Default.Publish(new ChatLoginPayload("maxim", "pass"));
		}

		private bool FilterMsg(ChatLoginPayload payload)
		{
			return payload.Username == "maxim";
		}

		private void OnChatLogin(ChatLoginPayload payload)
		{
#if UNITY_EDITOR && !UNITY_WSA
			Assert.IsNotNull(payload);
			Assert.IsNotNull(payload.Password);
#endif
		}
	}

	public class ChatLoginPayload : BaseChatPayload
	{
		public string Username { get; set; }

		public string Password { get; set; }

		public ChatLoginPayload(string username, string pasword)
		{
			Username = username;
			Password = pasword;
		}

		public override string ToString()
		{
			return string.Format("{0}, {1}", Username, Password);
		}
	}

	public class BaseChatPayload
	{
	}
}