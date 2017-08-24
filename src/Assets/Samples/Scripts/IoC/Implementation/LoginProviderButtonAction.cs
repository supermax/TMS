using TMS.Common.Messaging;
using TMS.Runtime.Unity.Actions;
using UnityEngine;

public abstract class LoginProviderButtonAction<T> : GameObjectAction
	where T : LoginProvider
{
	[SerializeField]
	private LoginProviderPayload<T> _loginProviderPayload;

	public virtual LoginProviderPayload<T> LoginProviderPayload
	{
		get { return _loginProviderPayload;  }
		protected set { _loginProviderPayload = value; }
	}

	protected override void DoActionInternal()
	{
		base.DoActionInternal();

		if(LoginProviderPayload == null) return; // TODO log?

		Messenger.Default.Publish<LoginProviderPayload>(LoginProviderPayload);
	}
}