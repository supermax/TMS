using TMS.Common.Core;
using TMS.Common.Messaging;
using TMS.Common.Modularity;

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
		var provider = IocManager.Default.Resolve<ILoginProvider>(payload.LoginProviderType);
		provider.Login();
	}
}