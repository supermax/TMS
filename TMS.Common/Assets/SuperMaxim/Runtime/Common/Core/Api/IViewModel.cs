#region Usings



#endregion

namespace TMS.Common.Core
{
	/// <summary>
	///     Interface for ViewModel
	/// </summary>
	public interface IViewModel : IObservable
	{
		/// <summary>
		///     Gets the data items.
		/// </summary>
		DataItemsContainer DataItems { get; }

		/// <summary>
		///     Gets the commands.
		/// </summary>
		CommandsContainer Commands { get; }

		/// <summary>
		///     Suspends property observation and begins the initialization session
		///     <para>Ensure calling EndInit() after update</para>
		/// </summary>
		void BeginInit();

		/// <summary>
		///     Ends the initialization session and resolves property observation
		/// </summary>
		/// <param name="isSilentUpdate">
		///     if set to <c>true</c> [the properties' change event will not be raised].
		/// </param>
		void EndInit(bool isSilentUpdate = false);

		/// <summary>
		///     Resets value in some properties (back to defaults).
		/// </summary>
		void Reset();
	}
}