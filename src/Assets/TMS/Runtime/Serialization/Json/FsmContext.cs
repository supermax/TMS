namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     FSM Context
	/// </summary>
	internal class FsmContext
	{
		/// <summary>
		///     The Lexer
		/// </summary>
		public Lexer Lex;

		/// <summary>
		///     The next state
		/// </summary>
		public int NextState;

		/// <summary>
		///     The return
		/// </summary>
		public bool Return;

		/// <summary>
		///     The state stack
		/// </summary>
		public int StateStack;
	}
}