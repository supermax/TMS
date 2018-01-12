#region Usings

using System;
using System.Threading;
using TMS.Common.Core;
using TMS.Common.Logging;
using TMS.Common.Tasks.Threading;
using UnityEngine;
using UnityEngine.UI;

#endregion

[Serializable]
public class ChatManager : ViewModel
{
	public InputField ChatInputField;

	public Text ChatTextLabel;

	public Toggle FilterToggle;

	public Text UserNameLabel;

	private static int _instanceCount;

    protected override void Awake()
	{
		base.Awake();

		Id = string.Format("USER_{0}", ++_instanceCount);

        Loggers.Default.ConsoleLogger.Init();        
	}

    private void OnMyEvent()
    {
        gameObject.AddComponent<Button>();
        Debug.LogWarning("Called from thread ID: " + Thread.CurrentThread.ManagedThreadId);
    }

    protected override void Start()
	{
		base.Start();

		UserNameLabel.text = Id;
	}

	public override void Subscribe()
	{
		base.Subscribe();

		Subscribe<IChatMessage>(OnChatMessageReceived, CanReceive);
	}

    protected override void OnDestroy()
    {
        Unsubscribe<IChatMessage>(OnChatMessageReceived);
        base.OnDestroy();
    }

    private bool CanReceive(IChatMessage payload)
	{
		if (FilterToggle.isOn == false)
		{
			return true;
		}

		var canReceive = payload.SenderId != Id;
		return canReceive;
	}

	private void OnChatMessageReceived(IChatMessage payload)
	{
		var msg = string.Format("[{0}]: {1}", payload.SenderId, payload.Text);
		ChatTextLabel.text += string.Format("{0}\r\n", msg);
		Debug.Log(msg);
	}

	public void SendText()
	{
		var txt = ChatInputField.text.Trim();
        ChatInputField.text = string.Empty;

        //new Thread(() => Publish<IChatMessage>(new ChatMessagePayload {SenderId = Id, Text = txt})).Start();

	    Publish<IChatMessage>(new ChatMessagePayload {SenderId = Id, Text = txt});

        //InvokeTest();
	}

    public void InvokeTest()
    {
        Debug.LogFormat("ManagedThreadId: {0}, Proxy: {1}", Thread.CurrentThread.ManagedThreadId, ThreadHelper.Default.CurrentDispatcher);

		ThreadHelper.Default.CurrentDispatcher.Dispatch(OnNewThread);
    }

    private void OnNewThread()
    {
        Debug.LogFormat("OnNewThread: {0}", Thread.CurrentThread.ManagedThreadId);

        ThreadHelper.Default.CurrentDispatcher.Dispatch(OnMainThread);
    }

    private void OnMainThread()
    {
        Debug.LogFormat("OnMainThread: {0}", Thread.CurrentThread.ManagedThreadId);        
    }
}