namespace TMS.Common.Serialization.Json
{
	internal enum ParserToken
	{
		/// <summary>
		///     Lexer tokens
		/// </summary>
		None = System.Char.MaxValue + 1,

		/// <summary>
		///     The number
		/// </summary>
		Number,

		/// <summary>
		///     The true
		/// </summary>
		True,

		/// <summary>
		///     The false
		/// </summary>
		False,

		/// <summary>
		///     The null
		/// </summary>
		Null,

		/// <summary>
		///     The char seq
		/// </summary>
		CharSeq,

		/// <summary>
		///     Single char
		/// </summary>
		Char,

		/// <summary>
		///     Parser Rules
		/// </summary>
		Text,

		/// <summary>
		///     The object
		/// </summary>
		Object,

		/// <summary>
		///     The object prime
		/// </summary>
		ObjectPrime,

		/// <summary>
		///     The pair
		/// </summary>
		Pair,

		/// <summary>
		///     The pair rest
		/// </summary>
		PairRest,

		/// <summary>
		///     The array
		/// </summary>
		Array,

		/// <summary>
		///     The array prime
		/// </summary>
		ArrayPrime,

		/// <summary>
		///     The value
		/// </summary>
		Value,

		/// <summary>
		///     The value rest
		/// </summary>
		ValueRest,

		/// <summary>
		///     The string
		/// </summary>
		String,

		/// <summary>
		///     End of input
		/// </summary>
		End,

		/// <summary>
		///     The empty rule
		/// </summary>
		Epsilon
	}
}