#region

using System;
using System.IO;
using System.Text;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Lexer
	/// </summary>
	internal class Lexer
	{
		#region Fields

		private static int[] _fsmReturnTable;
		private static StateHandler[] _fsmHandlerTable;
		private readonly FsmContext _fsmContext;
		private readonly TextReader _reader;
		private readonly StringBuilder _stringBuffer;

		private int _inputBuffer;
		private int _inputChar;
		private int _state;
		private int _unichar;

		private delegate bool StateHandler(FsmContext ctx);

		#endregion

		#region Properties

		/// <summary>
		///     Gets or sets a value indicating whether [allow comments].
		/// </summary>
		/// <value>
		///     <c>true</c> if [allow comments]; otherwise, <c>false</c>.
		/// </value>
		public bool AllowComments { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether [allow single quoted strings].
		/// </summary>
		/// <value>
		///     <c>true</c> if [allow single quoted strings]; otherwise, <c>false</c>.
		/// </value>
		public bool AllowSingleQuotedStrings { get; set; }

		/// <summary>
		///     Gets a value indicating whether [end of input].
		/// </summary>
		/// <value>
		///     <c>true</c> if [end of input]; otherwise, <c>false</c>.
		/// </value>
		public bool EndOfInput { get; private set; }

		/// <summary>
		///     Gets the token.
		/// </summary>
		/// <value>
		///     The token.
		/// </value>
		public int Token { get; private set; }

		/// <summary>
		///     Gets the string value.
		/// </summary>
		/// <value>
		///     The string value.
		/// </value>
		public string StringValue { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		///     Initializes the <see cref="Lexer" /> class.
		/// </summary>
		static Lexer()
		{
			PopulateFsmTables();
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="Lexer" /> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public Lexer(TextReader reader)
		{
			AllowComments = true;
			AllowSingleQuotedStrings = true;

			_inputBuffer = 0;
			_stringBuffer = new StringBuilder(128);
			_state = 1;
			EndOfInput = false;
			_reader = reader;

			_fsmContext = new FsmContext {Lex = this};
		}

		#endregion

		#region Static Methods

		/// <summary>
		///     Hexes the value.
		/// </summary>
		/// <param name="digit">The digit.</param>
		/// <returns></returns>
		private static int HexValue(int digit)
		{
			switch (digit)
			{
				case 'a':
				case 'A':
					return 10;

				case 'b':
				case 'B':
					return 11;

				case 'c':
				case 'C':
					return 12;

				case 'd':
				case 'D':
					return 13;

				case 'e':
				case 'E':
					return 14;

				case 'f':
				case 'F':
					return 15;

				default:
					return digit - '0';
			}
		}

		/// <summary>
		///     Populates the FSM tables.
		/// </summary>
		private static void PopulateFsmTables()
		{
			_fsmHandlerTable = new StateHandler[]
			{
				State1,
				State2,
				State3,
				State4,
				State5,
				State6,
				State7,
				State8,
				State9,
				State10,
				State11,
				State12,
				State13,
				State14,
				State15,
				State16,
				State17,
				State18,
				State19,
				State20,
				State21,
				State22,
				State23,
				State24,
				State25,
				State26,
				State27,
				State28
			};

			_fsmReturnTable = new[]
			{
				(int) ParserToken.Char,
				0,
				(int) ParserToken.Number,
				(int) ParserToken.Number,
				0,
				(int) ParserToken.Number,
				0,
				(int) ParserToken.Number,
				0,
				0,
				(int) ParserToken.True,
				0,
				0,
				0,
				(int) ParserToken.False,
				0,
				0,
				(int) ParserToken.Null,
				(int) ParserToken.CharSeq,
				(int) ParserToken.Char,
				0,
				0,
				(int) ParserToken.CharSeq,
				(int) ParserToken.Char,
				0,
				0,
				0,
				0
			};
		}

		/// <summary>
		///     Processes the esc char.
		/// </summary>
		/// <param name="escChar">The esc char.</param>
		/// <returns></returns>
		private static char ProcessEscChar(int escChar)
		{
			switch (escChar)
			{
				case '"':
				case '\'':
				case '\\':
				case '/':
					return Convert.ToChar(escChar);

				case 'n':
					return '\n';

				case 't':
					return '\t';

				case 'r':
					return '\r';

				case 'b':
					return '\b';

				case 'f':
					return '\f';

				default:
					// Unreachable
					return '?';
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State1(FsmContext ctx)
		{
			while (ctx.Lex.GetChar())
			{
				if (ctx.Lex._inputChar == ' ' ||
				    ctx.Lex._inputChar >= '\t' && ctx.Lex._inputChar <= '\r')
					continue;

				if (ctx.Lex._inputChar >= '1' && ctx.Lex._inputChar <= '9')
				{
					ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
					ctx.NextState = 3;
					return true;
				}

				switch (ctx.Lex._inputChar)
				{
					case '"':
						ctx.NextState = 19;
						ctx.Return = true;
						return true;

					case ',':
					case ':':
					case '[':
					case ']':
					case '{':
					case '}':
						ctx.NextState = 1;
						ctx.Return = true;
						return true;

					case '-':
						ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
						ctx.NextState = 2;
						return true;

					case '0':
						ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
						ctx.NextState = 4;
						return true;

					case 'f':
						ctx.NextState = 12;
						return true;

					case 'n':
						ctx.NextState = 16;
						return true;

					case 't':
						ctx.NextState = 9;
						return true;

					case '\'':
						if (! ctx.Lex.AllowSingleQuotedStrings)
							return false;

						ctx.Lex._inputChar = '"';
						ctx.NextState = 23;
						ctx.Return = true;
						return true;

					case '/':
						if (! ctx.Lex.AllowComments)
							return false;

						ctx.NextState = 25;
						return true;

					default:
						return false;
				}
			}

			return true;
		}

		private static bool State2(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			if (ctx.Lex._inputChar >= '1' && ctx.Lex._inputChar <= '9')
			{
				ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
				ctx.NextState = 3;
				return true;
			}

			switch (ctx.Lex._inputChar)
			{
				case '0':
					ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
					ctx.NextState = 4;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State3(FsmContext ctx)
		{
			while (ctx.Lex.GetChar())
			{
				if (ctx.Lex._inputChar >= '0' && ctx.Lex._inputChar <= '9')
				{
					ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
					continue;
				}

				if (ctx.Lex._inputChar == ' ' ||
				    ctx.Lex._inputChar >= '\t' && ctx.Lex._inputChar <= '\r')
				{
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
				}

				switch (ctx.Lex._inputChar)
				{
					case ',':
					case ']':
					case '}':
						ctx.Lex.UngetChar();
						ctx.Return = true;
						ctx.NextState = 1;
						return true;

					case '.':
						ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
						ctx.NextState = 5;
						return true;

					case 'e':
					case 'E':
						ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
						ctx.NextState = 7;
						return true;

					default:
						return false;
				}
			}
			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State4(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			if (ctx.Lex._inputChar == ' ' ||
			    ctx.Lex._inputChar >= '\t' && ctx.Lex._inputChar <= '\r')
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}

			switch (ctx.Lex._inputChar)
			{
				case ',':
				case ']':
				case '}':
					ctx.Lex.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;

				case '.':
					ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
					ctx.NextState = 5;
					return true;

				case 'e':
				case 'E':
					ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
					ctx.NextState = 7;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State5(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			if (ctx.Lex._inputChar < '0' || ctx.Lex._inputChar > '9') return false;
			ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
			ctx.NextState = 6;
			return true;
		}

		/// <summary>
		///     S
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State6(FsmContext ctx)
		{
			while (ctx.Lex.GetChar())
			{
				if (ctx.Lex._inputChar >= '0' && ctx.Lex._inputChar <= '9')
				{
					ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
					continue;
				}

				if (ctx.Lex._inputChar == ' ' ||
				    ctx.Lex._inputChar >= '\t' && ctx.Lex._inputChar <= '\r')
				{
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
				}

				switch (ctx.Lex._inputChar)
				{
					case ',':
					case ']':
					case '}':
						ctx.Lex.UngetChar();
						ctx.Return = true;
						ctx.NextState = 1;
						return true;

					case 'e':
					case 'E':
						ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
						ctx.NextState = 7;
						return true;

					default:
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State7(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			if (ctx.Lex._inputChar >= '0' && ctx.Lex._inputChar <= '9')
			{
				ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
				ctx.NextState = 8;
				return true;
			}

			switch (ctx.Lex._inputChar)
			{
				case '+':
				case '-':
					ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
					ctx.NextState = 8;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State8(FsmContext ctx)
		{
			while (ctx.Lex.GetChar())
			{
				if (ctx.Lex._inputChar >= '0' && ctx.Lex._inputChar <= '9')
				{
					ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
					continue;
				}

				if (ctx.Lex._inputChar == ' ' ||
				    ctx.Lex._inputChar >= '\t' && ctx.Lex._inputChar <= '\r')
				{
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
				}

				switch (ctx.Lex._inputChar)
				{
					case ',':
					case ']':
					case '}':
						ctx.Lex.UngetChar();
						ctx.Return = true;
						ctx.NextState = 1;
						return true;

					default:
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State9(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'r':
					ctx.NextState = 10;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State10(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'u':
					ctx.NextState = 11;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State11(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'e':
					ctx.Return = true;
					ctx.NextState = 1;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State12(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'a':
					ctx.NextState = 13;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State13(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'l':
					ctx.NextState = 14;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State14(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 's':
					ctx.NextState = 15;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State15(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'e':
					ctx.Return = true;
					ctx.NextState = 1;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State16(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'u':
					ctx.NextState = 17;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State17(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'l':
					ctx.NextState = 18;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State18(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'l':
					ctx.Return = true;
					ctx.NextState = 1;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State19(FsmContext ctx)
		{
			while (ctx.Lex.GetChar())
			{
				switch (ctx.Lex._inputChar)
				{
					case '"':
						ctx.Lex.UngetChar();
						ctx.Return = true;
						ctx.NextState = 20;
						return true;

					case '\\':
						ctx.StateStack = 19;
						ctx.NextState = 21;
						return true;

					default:
						ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
						continue;
				}
			}

			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State20(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case '"':
					ctx.Return = true;
					ctx.NextState = 1;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State21(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case 'u':
					ctx.NextState = 22;
					return true;

				case '"':
				case '\'':
				case '/':
				case '\\':
				case 'b':
				case 'f':
				case 'n':
				case 'r':
				case 't':
					ctx.Lex._stringBuffer.Append(
						ProcessEscChar(ctx.Lex._inputChar));
					ctx.NextState = ctx.StateStack;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State22(FsmContext ctx)
		{
			var counter = 0;
			var mult = 4096;

			ctx.Lex._unichar = 0;

			while (ctx.Lex.GetChar())
			{
				if ((ctx.Lex._inputChar < '0' || ctx.Lex._inputChar > '9') && (ctx.Lex._inputChar < 'A' || ctx.Lex._inputChar > 'F') &&
				    (ctx.Lex._inputChar < 'a' || ctx.Lex._inputChar > 'f'))
				{
					return false;
				}
				ctx.Lex._unichar += HexValue(ctx.Lex._inputChar)*mult;

				counter++;
				mult /= 16;

				if (counter != 4) continue;

				ctx.Lex._stringBuffer.Append(Convert.ToChar(ctx.Lex._unichar));
				ctx.NextState = ctx.StateStack;
				return true;
			}

			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State23(FsmContext ctx)
		{
			while (ctx.Lex.GetChar())
			{
				switch (ctx.Lex._inputChar)
				{
					case '\'':
						ctx.Lex.UngetChar();
						ctx.Return = true;
						ctx.NextState = 24;
						return true;

					case '\\':
						ctx.StateStack = 23;
						ctx.NextState = 21;
						return true;

					default:
						ctx.Lex._stringBuffer.Append((char) ctx.Lex._inputChar);
						continue;
				}
			}

			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State24(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case '\'':
					ctx.Lex._inputChar = '"';
					ctx.Return = true;
					ctx.NextState = 1;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State25(FsmContext ctx)
		{
			ctx.Lex.GetChar();

			switch (ctx.Lex._inputChar)
			{
				case '*':
					ctx.NextState = 27;
					return true;

				case '/':
					ctx.NextState = 26;
					return true;

				default:
					return false;
			}
		}

		/// <summary>
		///     State26s the specified CTX.
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State26(FsmContext ctx)
		{
			while (ctx.Lex.GetChar())
			{
				if (ctx.Lex._inputChar != '\n') continue;
				ctx.NextState = 1;
				return true;
			}

			return true;
		}

		private static bool State27(FsmContext ctx)
		{
			while (ctx.Lex.GetChar())
			{
				if (ctx.Lex._inputChar != '*') continue;
				ctx.NextState = 28;
				return true;
			}

			return true;
		}

		/// <summary>
		/// </summary>
		/// <param name="ctx">The CTX.</param>
		/// <returns></returns>
		private static bool State28(FsmContext ctx)
		{
			while (ctx.Lex.GetChar())
			{
				if (ctx.Lex._inputChar == '*')
				{
					continue;
				}

				if (ctx.Lex._inputChar == '/')
				{
					ctx.NextState = 1;
					return true;
				}

				ctx.NextState = 27;
				return true;
			}

			return true;
		}

		#endregion

		/// <summary>
		///     Gets the char.
		/// </summary>
		/// <returns></returns>
		private bool GetChar()
		{
			if ((_inputChar = NextChar()) != -1)
				return true;

			EndOfInput = true;
			return false;
		}

		/// <summary>
		///     Nexts the char.
		/// </summary>
		/// <returns></returns>
		private int NextChar()
		{
			if (_inputBuffer == 0)
			{
				var idx = _reader.Read();
				return idx;
			}
			var tmp = _inputBuffer;
			_inputBuffer = 0;
			return tmp;
		}

		/// <summary>
		///     Nexts the token.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="TMS.Common.Serialization.Json.JsonException"></exception>
		public bool NextToken()
		{
			_fsmContext.Return = false;

			while (true)
			{
				var handler = _fsmHandlerTable[_state - 1];
				if (! handler(_fsmContext))
				{
					throw new JsonException(_inputChar);
				}
				if (EndOfInput)
				{
					return false;
				}

				if (_fsmContext.Return)
				{
					StringValue = _stringBuffer.ToString();
					_stringBuffer.Remove(0, _stringBuffer.Length);
					Token = _fsmReturnTable[_state - 1];

					if (Token == (int) ParserToken.Char)
					{
						Token = _inputChar;
					}
					_state = _fsmContext.NextState;
					return true;
				}

				_state = _fsmContext.NextState;
			}
		}

		/// <summary>
		///     Ungets the char.
		/// </summary>
		private void UngetChar()
		{
			_inputBuffer = _inputChar;
		}
	}
}