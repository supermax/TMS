using System;
using TMS.Common.Helpers;

namespace TMS.Common.EventArguments
{
	/// <summary>
	/// 
	/// </summary>
	public class InputEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the type of the input.
		/// </summary>
		/// <value>
		/// The type of the input.
		/// </value>
		public InputTypes InputType { get; set; }
	}
}