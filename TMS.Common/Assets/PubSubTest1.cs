using System.Collections;
using System.Collections.Generic;
using TMS.Common.Messaging;
using UnityEngine;

public class PubSubTest1 : MonoBehaviour {

	public class MyMsg
	{
		public string Txt { get; set; }
	}
	
	// Use this for initialization
	void Start ()
	{
		Messenger.Default.Subscribe<MyMsg>(OnMyMsg);
	}

	private void OnMyMsg(MyMsg msg)
	{
		Debug.LogFormat("My Msg: {0}", msg.Txt);
	}

	private int _count;

	private void OnDisable()
	{
		Messenger.Default.Unsubscribe<MyMsg>(OnMyMsg);
	}

	// Update is called once per frame
	void Update () {

		if (_count > 8)
		{
			

			enabled = false;
			
			return;
		}
		
		
		
		
		Messenger.Default.Publish(new MyMsg { Txt = (++_count) + "_Blah"});
		
		
			
		
	}
}
