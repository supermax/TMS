using System;
using System.Collections;
using System.Collections.Generic;
using TMS.Common.Core;
using UnityEngine;

[TMS.Common.Modularity.IocTypeMap(typeof(MessagePrinter), 
	true, InstantiateOnRegistration = true)]
public class MessagePrinter : ViewModelSingleton<IMessagePrinter, MessagePrinter>, IMessagePrinter
{
	protected override void OnEnable()
	{
		base.OnEnable();

		Subscribe<IMessage>(OnMessageReceived);
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		Unsubscribe<IMessage>(OnMessageReceived);
	}

	public void OnMessageReceived(IMessage msg)
	{
		base.Log("[{0}] {1}", DateTime.Now.TimeOfDay, msg.Message);
	}
}

public interface IMessagePrinter : IViewModel
{
	void OnMessageReceived(IMessage msg);
}
