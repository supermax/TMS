#region Usings

using System;

#endregion

namespace TMS.Common.AppManager
{
	/// <summary>
	///     Application Instance Callback Event Arguments
	/// </summary>
	public class AppInstanceCallbackEventArgs : EventArgs
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="AppInstanceCallbackEventArgs" /> class.
		/// </summary>
		/// <param name="isFirstInstance">
		///     if set to <c>true</c> [is first instance].
		/// </param>
		/// <param name="commandLineArgs">The command line args.</param>
		public AppInstanceCallbackEventArgs(bool isFirstInstance, string[] commandLineArgs)
		{
			IsFirstInstance = isFirstInstance;
			CommandLineArgs = commandLineArgs;
		}

		/// <summary>
		///     Gets a value indicating whether this instance is first instance.
		/// </summary>
		/// <value>
		///     <c>true</c> if this instance is first instance; otherwise, <c>false</c>.
		/// </value>
		public bool IsFirstInstance { get; private set; }

		/// <summary>
		///     Gets the command line args.
		/// </summary>
		/// <value>The command line args.</value>
		public string[] CommandLineArgs { get; private set; }
	}
}