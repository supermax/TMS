using System;
using TMS.Runtime.Unity.Actions;
using UnityEngine;

[Serializable]
public abstract class LoginProviderPayload : MessengerActionPayload
{
	public abstract Type LoginProviderType { get; }
}

[Serializable]
public class LoginProviderPayload<T> : LoginProviderPayload
	where T : LoginProvider
{
	public override Type LoginProviderType
	{
		get { return typeof(T); }
	}
}