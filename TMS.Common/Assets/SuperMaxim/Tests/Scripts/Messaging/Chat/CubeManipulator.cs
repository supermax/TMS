using System;
using System.Collections.Generic;
using TMS.Common.Core;
using TMS.Common.Messaging;
using UnityEngine;

[MessengerConsumer(typeof(ICubeManipulator), true, InstantiateOnRegistration = false, AutoSubscribe = true)]
public class CubeManipulator : MonoBehaviorBaseSingleton<CubeManipulator>, ICubeManipulator, IMessengerConsumer
{
	private IDictionary<string, Action> _actions;

	protected override void Start()
	{
		_actions = new Dictionary<string, Action>
						{
							{ "#cube_on",  ShowCube},
							{ "#cube_off",  HideCube},
							{ "#cube_rotate",  RotateCube}
						};

		base.Start();

	    //GameManager.Default.UserName.text = "CUBE";
     }

	public void Subscribe()
	{
		Messenger.Default.Subscribe<IChatMessage>(OnChatMessageReceived,
			msg => _actions.ContainsKey(msg.Text));
	}

	private void OnChatMessageReceived(IChatMessage payload)
	{
		var act = _actions[payload.Text];
		act();
		Debug.LogFormat("{0}->{1}", payload.Text, act);
	}

	public void ShowCube()
	{
		gameObject.SetActive(true);
	}

	public void HideCube()
	{
		gameObject.SetActive(false);
	}

	public void RotateCube()
	{
		gameObject.transform.Rotate(new Vector3(0, 1, 0), 30);
	}
}

public interface ICubeManipulator
{
	void ShowCube();

	void HideCube();

	void RotateCube();
}