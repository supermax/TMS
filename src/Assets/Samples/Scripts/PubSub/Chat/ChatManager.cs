﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMS.Common.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : BaseManager
{
	[SerializeField]
	private Text _userNameText;

	[SerializeField]
	private Text _chatText;

	[SerializeField]
	private InputField _inputText;

	protected override void Start()
	{
		base.Start();

		_chatText.text = string.Empty;
		_userNameText.text = string.Format("USER #{0}", InstanceId);
	}

	public void Send(string message)
	{
		if (_inputText.text.IsNullOrEmpty()) return;

		Publish(new ChatMessagePayload(InstanceId, _inputText.text));
				
		_inputText.text = string.Empty;
	}
		
	public override void Subscribe()
	{
		Subscribe<ChatMessagePayload>(OnChatMessageReceived, p => p.SenderId != InstanceId);

		base.Subscribe();
	}

	public override void Unsubscribe()
	{
		Unsubscribe<ChatMessagePayload>(OnChatMessageReceived);

		base.Unsubscribe();		
	}

	private void OnChatMessageReceived(ChatMessagePayload payload)
	{
		_chatText.text += string.Format("\n[USER #{0}]: {1}", payload.SenderId, payload.Message);
	}
}
