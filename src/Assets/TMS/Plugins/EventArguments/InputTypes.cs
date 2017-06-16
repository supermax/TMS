using System;

namespace TMS.Common.EventArguments
{
	/// <summary>
	/// Input Types (Enum)
	/// </summary>
	[Flags]
	public enum InputTypes
	{
		/// <summary>
		/// None
		/// </summary>
		None = 0x0,

		/// <summary>
		/// The touch down
		/// </summary>
		TouchDown = 0x1,

		/// <summary>
		/// The touch up
		/// </summary>
		TouchUp = 0x2,

		/// <summary>
		/// The mouse down
		/// </summary>
		MouseDown = 0x4,

		/// <summary>
		/// The mouse up
		/// </summary>
		MouseUp = 0x8,
	}
}