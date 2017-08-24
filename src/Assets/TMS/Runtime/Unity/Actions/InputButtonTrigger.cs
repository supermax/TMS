#region Usings

using System;

#endregion

namespace TMS.Runtime.Unity.Actions
{
	[Serializable]
	public enum InputButtonTrigger
	{
		None = 0,
		KeyDown,
		KeyUp,
		ButtonDown,
		ButtonUp
	}
}