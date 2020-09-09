using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TMS.Runtime.Unity.Actions
{
	[Serializable]
	public class MessengerActionPayload
	{
		[SerializeField]
		private Object _target;

		public virtual Object Target
		{
			get { return _target; }
			protected set { _target = value; }
		}

		[SerializeField]
		private Object[] _arguments;

		public virtual Object[] Arguments
		{
			get { return _arguments; }
			protected set { _arguments = value; }
		}

		public MessengerActionPayload()
		{
			
		}

		public MessengerActionPayload(Object target, params Object[] args)
		{
			Target = target;
			Arguments = args;
		}
	}
}