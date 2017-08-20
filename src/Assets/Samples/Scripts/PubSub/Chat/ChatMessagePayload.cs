using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatMessagePayload
{
	public string Message { get; private set; }

	public int SenderId { get; private set; }

	public ChatMessagePayload(int senderId, string message)
	{
		SenderId = senderId;
		Message = message;
	}
}
