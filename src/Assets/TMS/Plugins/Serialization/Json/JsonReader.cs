#region

using System;
using System.Collections.Generic;
using System.IO;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Json Reader
	/// </summary>
	public class JsonReader
	{
		#region Fields

		private static IDictionary<int, IDictionary<int, int[]>> _parseTable;

		private readonly Stack<int> _automatonStack;
		private readonly Lexer _lexer;
		private readonly bool _readerIsOwned;
		private int _currentInput;
		private int _currentSymbol;
		private bool _parserInString;
		private bool _parserReturn;
		private bool _readStarted;
		private TextReader _reader;

		#endregion

		#region Public Properties

		/// <summary>
		///     Gets or sets a value indicating whether [allow comments].
		/// </summary>
		/// <value>
		///     <c>true</c> if [allow comments]; otherwise, <c>false</c>.
		/// </value>
		public bool AllowComments
		{
			get { return _lexer.AllowComments; }
			set { _lexer.AllowComments = value; }
		}

		/// <summary>
		///     Gets or sets a value indicating whether [allow single quoted strings].
		/// </summary>
		/// <value>
		///     <c>true</c> if [allow single quoted strings]; otherwise, <c>false</c>.
		/// </value>
		public bool AllowSingleQuotedStrings
		{
			get { return _lexer.AllowSingleQuotedStrings; }
			set { _lexer.AllowSingleQuotedStrings = value; }
		}

		/// <summary>
		///     Gets a value indicating whether [end of input].
		/// </summary>
		/// <value>
		///     <c>true</c> if [end of input]; otherwise, <c>false</c>.
		/// </value>
		public bool EndOfInput { get; private set; }

		/// <summary>
		///     Gets a value indicating whether [end of json].
		/// </summary>
		/// <value>
		///     <c>true</c> if [end of json]; otherwise, <c>false</c>.
		/// </value>
		public bool EndOfJson { get; private set; }

		/// <summary>
		///     Gets the token.
		/// </summary>
		/// <value>
		///     The token.
		/// </value>
		public JsonToken Token { get; private set; }

		/// <summary>
		///     Gets the value.
		/// </summary>
		/// <value>
		///     The value.
		/// </value>
		public object Value { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		///     Initializes the <see cref="JsonReader" /> class.
		/// </summary>
		static JsonReader()
		{
			PopulateParseTable();
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonReader" /> class.
		/// </summary>
		/// <param name="jsonText">The json text.</param>
		public JsonReader(string jsonText) :
			this(new StringReader(jsonText), true)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonReader" /> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public JsonReader(TextReader reader) :
			this(reader, false)
		{
		}

		/// <summary>
		///     Prevents a default instance of the <see cref="JsonReader" /> class from being created.
		/// </summary>
		/// <param name="reader">The reader.</param>
		/// <param name="owned">if set to <c>true</c> [owned].</param>
		/// <exception cref="System.ArgumentNullException">reader</exception>
		private JsonReader(TextReader reader, bool owned)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}

			_parserInString = false;
			_parserReturn = false;

			_readStarted = false;
			_automatonStack = new Stack<int>();
			_automatonStack.Push((int) ParserToken.End);
			_automatonStack.Push((int) ParserToken.Text);

			_lexer = new Lexer(reader);

			EndOfInput = false;
			EndOfJson = false;

			_reader = reader;
			_readerIsOwned = owned;
		}

		#endregion

		#region Static Methods

		/// <summary>
		///     Populates the parse table.
		/// </summary>
		private static void PopulateParseTable()
		{
			_parseTable = new Dictionary<int, IDictionary<int, int[]>>();

			TableAddRow(ParserToken.Array);
			TableAddCol(ParserToken.Array, '[',
				'[',
				(int) ParserToken.ArrayPrime);

			TableAddRow(ParserToken.ArrayPrime);
			TableAddCol(ParserToken.ArrayPrime, '"',
				(int) ParserToken.Value,
				(int) ParserToken.ValueRest,
				']');
			TableAddCol(ParserToken.ArrayPrime, '[',
				(int) ParserToken.Value,
				(int) ParserToken.ValueRest,
				']');
			TableAddCol(ParserToken.ArrayPrime, ']',
				']');
			TableAddCol(ParserToken.ArrayPrime, '{',
				(int) ParserToken.Value,
				(int) ParserToken.ValueRest,
				']');
			TableAddCol(ParserToken.ArrayPrime, (int) ParserToken.Number,
				(int) ParserToken.Value,
				(int) ParserToken.ValueRest,
				']');
			TableAddCol(ParserToken.ArrayPrime, (int) ParserToken.True,
				(int) ParserToken.Value,
				(int) ParserToken.ValueRest,
				']');
			TableAddCol(ParserToken.ArrayPrime, (int) ParserToken.False,
				(int) ParserToken.Value,
				(int) ParserToken.ValueRest,
				']');
			TableAddCol(ParserToken.ArrayPrime, (int) ParserToken.Null,
				(int) ParserToken.Value,
				(int) ParserToken.ValueRest,
				']');

			TableAddRow(ParserToken.Object);
			TableAddCol(ParserToken.Object, '{',
				'{',
				(int) ParserToken.ObjectPrime);

			TableAddRow(ParserToken.ObjectPrime);
			TableAddCol(ParserToken.ObjectPrime, '"',
				(int) ParserToken.Pair,
				(int) ParserToken.PairRest,
				'}');
			TableAddCol(ParserToken.ObjectPrime, '}',
				'}');

			TableAddRow(ParserToken.Pair);
			TableAddCol(ParserToken.Pair, '"',
				(int) ParserToken.String,
				':',
				(int) ParserToken.Value);

			TableAddRow(ParserToken.PairRest);
			TableAddCol(ParserToken.PairRest, ',',
				',',
				(int) ParserToken.Pair,
				(int) ParserToken.PairRest);
			TableAddCol(ParserToken.PairRest, '}',
				(int) ParserToken.Epsilon);

			TableAddRow(ParserToken.String);
			TableAddCol(ParserToken.String, '"',
				'"',
				(int) ParserToken.CharSeq,
				'"');

			TableAddRow(ParserToken.Text);
			TableAddCol(ParserToken.Text, '[',
				(int) ParserToken.Array);
			TableAddCol(ParserToken.Text, '{',
				(int) ParserToken.Object);

			TableAddRow(ParserToken.Value);
			TableAddCol(ParserToken.Value, '"',
				(int) ParserToken.String);
			TableAddCol(ParserToken.Value, '[',
				(int) ParserToken.Array);
			TableAddCol(ParserToken.Value, '{',
				(int) ParserToken.Object);
			TableAddCol(ParserToken.Value, (int) ParserToken.Number,
				(int) ParserToken.Number);
			TableAddCol(ParserToken.Value, (int) ParserToken.True,
				(int) ParserToken.True);
			TableAddCol(ParserToken.Value, (int) ParserToken.False,
				(int) ParserToken.False);
			TableAddCol(ParserToken.Value, (int) ParserToken.Null,
				(int) ParserToken.Null);

			TableAddRow(ParserToken.ValueRest);
			TableAddCol(ParserToken.ValueRest, ',',
				',',
				(int) ParserToken.Value,
				(int) ParserToken.ValueRest);
			TableAddCol(ParserToken.ValueRest, ']',
				(int) ParserToken.Epsilon);
		}

		private static void TableAddCol(ParserToken row, int col,
			params int[] symbols)
		{
			_parseTable[(int) row].Add(col, symbols);
		}

		private static void TableAddRow(ParserToken rule)
		{
			_parseTable.Add((int) rule, new Dictionary<int, int[]>());
		}

		#endregion

		#region Private Methods

		/// <summary>
		///     Processes the number.
		/// </summary>
		/// <param name="number">The number.</param>
		private void ProcessNumber(string number)
		{
			if (number.IndexOf('.') != -1 ||
			    number.IndexOf('e') != -1 ||
			    number.IndexOf('E') != -1)
			{
				double nDouble;
				if (Double.TryParse(number, out nDouble))
				{
					Token = JsonToken.Double;
					Value = nDouble;

					return;
				}
			}

			int nInt32;
			if (Int32.TryParse(number, out nInt32))
			{
				Token = JsonToken.Int;
				Value = nInt32;
				return;
			}

			long nInt64;
			if (Int64.TryParse(number, out nInt64))
			{
				Token = JsonToken.Long;
				Value = nInt64;

				return;
			}

			// Shouldn't happen, but just in case, return something
			Token = JsonToken.Int;
			Value = 0;
		}

		/// <summary>
		///     Processes the symbol.
		/// </summary>
		private void ProcessSymbol()
		{
			switch (_currentSymbol)
			{
				case '[':
					Token = JsonToken.ArrayStart;
					_parserReturn = true;
					break;

				case ']':
					Token = JsonToken.ArrayEnd;
					_parserReturn = true;
					break;

				case '{':
					Token = JsonToken.ObjectStart;
					_parserReturn = true;
					break;

				case '}':
					Token = JsonToken.ObjectEnd;
					_parserReturn = true;
					break;

				case '"':
					if (_parserInString)
					{
						_parserInString = false;

						_parserReturn = true;
					}
					else
					{
						if (Token == JsonToken.None)
							Token = JsonToken.String;

						_parserInString = true;
					}
					break;

				case (int) ParserToken.CharSeq:
					Value = _lexer.StringValue;
					break;

				case (int) ParserToken.False:
					Token = JsonToken.Boolean;
					Value = false;
					_parserReturn = true;
					break;

				case (int) ParserToken.Null:
					Token = JsonToken.Null;
					_parserReturn = true;
					break;

				case (int) ParserToken.Number:
					ProcessNumber(_lexer.StringValue);
					_parserReturn = true;
					break;

				case (int) ParserToken.Pair:
					Token = JsonToken.PropertyName;
					break;

				case (int) ParserToken.True:
					Token = JsonToken.Boolean;
					Value = true;
					_parserReturn = true;
					break;
			}
		}

		/// <summary>
		///     Reads the token.
		/// </summary>
		/// <returns></returns>
		private bool ReadToken()
		{
			if (EndOfInput)
			{
				return false;
			}
			_lexer.NextToken();

			if (_lexer.EndOfInput)
			{
				Close();
				return false;
			}

			_currentInput = _lexer.Token;
			return true;
		}

		#endregion

		/// <summary>
		///     Closes this instance.
		/// </summary>
		public void Close()
		{
			if (EndOfInput)
			{
				return;
			}

			EndOfInput = true;
			EndOfJson = true;

			if (!_readerIsOwned || _reader == null)
			{
				return;
			}
			_reader.Dispose();
			_reader = null;
		}

		/// <summary>
		///     Reads this instance.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="TMS.Common.Serialization.Json.JsonException">
		///     Input doesn't evaluate to proper JSON text
		///     or
		/// </exception>
		public bool Read()
		{
			if (EndOfInput)
			{
				return false;
			}

			if (EndOfJson)
			{
				EndOfJson = false;
				_automatonStack.Clear();
				_automatonStack.Push((int) ParserToken.End);
				_automatonStack.Push((int) ParserToken.Text);
			}

			_parserInString = false;
			_parserReturn = false;

			Token = JsonToken.None;
			Value = null;

			if (!_readStarted)
			{
				_readStarted = true;

				if (!ReadToken())
				{
					return false;
				}
			}

			while (true)
			{
				if (_parserReturn)
				{
					if (_automatonStack.Peek() == (int) ParserToken.End)
					{
						EndOfJson = true;
					}
					return true;
				}

				_currentSymbol = _automatonStack.Pop();
				ProcessSymbol();

				if (_currentSymbol == _currentInput)
				{
					if (!ReadToken())
					{
						if (_automatonStack.Peek() != (int) ParserToken.End)
						{
							throw new JsonException(
								"Input doesn't evaluate to proper JSON text");
						}
						return _parserReturn;
					}

					continue;
				}

				int[] entrySymbols;
				try
				{
					entrySymbols = _parseTable[_currentSymbol][_currentInput];
				}
				catch (KeyNotFoundException e)
				{
					throw new JsonException((ParserToken) _currentInput, e);
				}

				if (entrySymbols[0] == (int) ParserToken.Epsilon)
				{
					continue;
				}

				for (var i = entrySymbols.Length - 1; i >= 0; i--)
				{
					_automatonStack.Push(entrySymbols[i]);
				}
			}
		}
	}
}