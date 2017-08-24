﻿using TMS.Common.Core;
using TMS.Common.Messaging;

[MessengerConsumer(typeof(ILoginManager), true, 
	AutoSubscribe = true, InstantiateOnRegistration = true)]
public class LoginManager : Singleton<ILoginManager, LoginManager>, 
	ILoginManager, IMessengerConsumer
{
	public void Subscribe()
	{
		Messenger.Default.Subscribe<LoginProviderPayload>(OnLoginRequest);
	}

	private void OnLoginRequest(LoginProviderPayload payload)
	{
		
	}
}