using System;

namespace CFX.Breakout.Test.Common.Helpers
{
	[Serializable]
    public enum ObjectActionTrigger
	{
		None = 0,
		Awake,
		Start,
		OnEnable,
		OnDisable,
		OnDestroy,
		OnCollisionEnter
	}
}
