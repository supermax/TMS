﻿using TMS.Common.Messaging;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TMS.Runtime.Unity.Actions
{
	public class MessengerAction : GameObjectAction
	{
		[SerializeField]
		private MessengerActionPayload _payload;

		public virtual MessengerActionPayload Payload
		{
			get { return _payload; }
			protected set { _payload = value; }
		}

		[SerializeField]
		private Object[] _arguments;

		public virtual Object[] Arguments
		{
			get { return _arguments; }
			protected set { _arguments = value; }
		}

		[SerializeField]
		private bool _generatePayload;

		protected override void DoActionInternal()
		{
			base.DoActionInternal();

			var payload = Payload;
			if (payload == null)
			{
				if(!_generatePayload) return; // TODO log ?

				var target = GetTarget();
				payload = new MessengerActionPayload(target, Arguments);
			}
			Messenger.Default.Publish(payload);
		}
	}
}