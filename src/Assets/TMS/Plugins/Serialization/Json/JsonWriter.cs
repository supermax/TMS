#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	///     Json Writer
	/// </summary>
	public class JsonWriter : IDisposable
	{
		#region Fields

		private static readonly NumberFormatInfo NumberFormat;
		private readonly StringBuilder _instStringBuilder;

		private WriterContext _context;
		private Stack<WriterContext> _ctxStack;
		private bool _hasReachedEnd;
		private char[] _hexSeq;
		private int _indentValue;
		private int _indentation;
		private bool _disposed;

		#endregion

		#region Properties

		/// <summary>
		///     Gets or sets the indent value.
		/// </summary>
		/// <value>
		///     The indent value.
		/// </value>
		public int IndentValue
		{
			get { return _indentValue; }
			set
			{
				_indentation = (_indentation/_indentValue)*value;
				_indentValue = value;
			}
		}

		/// <summary>
		///     Gets or sets a value indicating whether [pretty print].
		/// </summary>
		/// <value>
		///     <c>true</c> if [pretty print]; otherwise, <c>false</c>.
		/// </value>
		public bool PrettyPrint { get; set; }

		/// <summary>
		///     Gets the text writer.
		/// </summary>
		/// <value>
		///     The text writer.
		/// </value>
		public TextWriter TextWriter { get; private set; }

		/// <summary>
		///     Gets or sets a value indicating whether this <see cref="JsonWriter" /> is validate.
		/// </summary>
		/// <value>
		///     <c>true</c> if validate; otherwise, <c>false</c>.
		/// </value>
		public bool Validate { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		///     Initializes the <see cref="JsonWriter" /> class.
		/// </summary>
		static JsonWriter()
		{
			NumberFormat = NumberFormatInfo.InvariantInfo;
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonWriter" /> class.
		/// </summary>
		public JsonWriter()
		{
			_instStringBuilder = new StringBuilder();
			TextWriter = new StringWriter(_instStringBuilder);

			Init();
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonWriter" /> class.
		/// </summary>
		/// <param name="sb">The sb.</param>
		public JsonWriter(StringBuilder sb) :
			this(new StringWriter(sb))
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonWriter" /> class.
		/// </summary>
		/// <param name="writer">The writer.</param>
		/// <exception cref="System.ArgumentNullException">writer</exception>
		public JsonWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			TextWriter = writer;
			Init();
		}

		#endregion

		#region Private Methods

		/// <summary>
		///     Does the validation.
		/// </summary>
		/// <param name="cond">The cond.</param>
		/// <exception cref="JsonException">
		///     A complete JSON symbol has already been written
		///     or
		///     Can't close an array here
		///     or
		///     Can't close an object here
		///     or
		///     Expected a property
		///     or
		///     Can't add a property here
		///     or
		///     Can't add a value here
		/// </exception>
		private void DoValidation(Condition cond)
		{
			if (! _context.ExpectingValue)
			{
				_context.Count++;
			}
			if (! Validate)
			{
				return;
			}
			if (_hasReachedEnd)
			{
				throw new JsonException(
					"A complete JSON symbol has already been written");
			}

			switch (cond)
			{
				case Condition.InArray:
					if (! _context.InArray)
					{
						throw new JsonException("Can't close an array here");
					}
					break;

				case Condition.InObject:
					if (! _context.InObject || _context.ExpectingValue)
					{
						throw new JsonException("Can't close an object here");
					}
					break;

				case Condition.NotAProperty:
					if (_context.InObject && ! _context.ExpectingValue)
					{
						throw new JsonException("Expected a property");
					}
					break;

				case Condition.Property:
					if (! _context.InObject || _context.ExpectingValue)
					{
						throw new JsonException("Can't add a property here");
					}
					break;

				case Condition.Value:
					if (! _context.InArray && (! _context.InObject || ! _context.ExpectingValue))
					{
						throw new JsonException("Can't add a value here");
					}
					break;
			}
		}

		/// <summary>
		///     Initializes this instance.
		/// </summary>
		private void Init()
		{
			_hasReachedEnd = false;
			_hexSeq = new char[4];
			_indentation = 0;
			_indentValue = 4;
			PrettyPrint = false;
			Validate = true;

			_ctxStack = new Stack<WriterContext>();
			_context = new WriterContext();
			_ctxStack.Push(_context);
		}

		/// <summary>
		///     INT to hex.
		/// </summary>
		/// <param name="n">The n.</param>
		/// <param name="hex">The hex.</param>
		private static void IntToHex(int n, IList<char> hex)
		{
			for (var i = 0; i < 4; i++)
			{
				var num = n%16;
				if (num < 10)
				{
					hex[3 - i] = (char) ('0' + num);
				}
				else
				{
					hex[3 - i] = (char) ('A' + (num - 10));
				}
				n >>= 4;
			}
		}

		/// <summary>
		///     Indents this instance.
		/// </summary>
		private void Indent()
		{
			if (PrettyPrint)
			{
				_indentation += _indentValue;
			}
		}

		/// <summary>
		///     Puts the specified STR.
		/// </summary>
		/// <param name="str">The STR.</param>
		private void Put(string str)
		{
			if (PrettyPrint && ! _context.ExpectingValue)
			{
				for (var i = 0; i < _indentation; i++)
				{
					TextWriter.Write(' ');
				}
			}
			TextWriter.Write(str);
		}

		/// <summary>
		///     Puts the newline.
		/// </summary>
		private void PutNewline()
		{
			PutNewline(true);
		}

		/// <summary>
		///     Puts the newline.
		/// </summary>
		/// <param name="addComma">if set to <c>true</c> [add comma].</param>
		private void PutNewline(bool addComma)
		{
			if (addComma && ! _context.ExpectingValue && _context.Count > 1)
			{
				TextWriter.Write(',');
			}

			if (PrettyPrint && ! _context.ExpectingValue)
			{
				TextWriter.Write('\n');
			}
		}

		/// <summary>
		///     Puts the string.
		/// </summary>
		/// <param name="str">The STR.</param>
		private void PutString(string str)
		{
			Put(String.Empty);

			TextWriter.Write('"');

			var n = str.Length;
			for (var i = 0; i < n; i++)
			{
				switch (str[i])
				{
					case '\n':
						TextWriter.Write("\\n");
						continue;

					case '\r':
						TextWriter.Write("\\r");
						continue;

					case '\t':
						TextWriter.Write("\\t");
						continue;

					case '"':
					case '\\':
						TextWriter.Write('\\');
						TextWriter.Write(str[i]);
						continue;

					case '\f':
						TextWriter.Write("\\f");
						continue;

					case '\b':
						TextWriter.Write("\\b");
						continue;
				}

				if (str[i] >= 32 && str[i] <= 126)
				{
					TextWriter.Write(str[i]);
					continue;
				}

				// Default, turn into a \uXXXX sequence
				IntToHex(str[i], _hexSeq);
				TextWriter.Write("\\u");
				TextWriter.Write(_hexSeq);
			}

			TextWriter.Write('"');
		}

		/// <summary>
		///     Unindents this instance.
		/// </summary>
		private void Unindent()
		{
			if (PrettyPrint)
			{
				_indentation -= _indentValue;
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(_disposed);
		}

		private void Dispose(bool disposed)
		{
			if(!disposed) return;
			if (TextWriter != null)
			{
				TextWriter.Dispose();
			}
			_disposed = true;
		}

		#endregion

		/// <summary>
		///     Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		///     A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return _instStringBuilder == null ? string.Empty : _instStringBuilder.ToString();
		}

		/// <summary>
		///     Resets this instance.
		/// </summary>
		public void Reset()
		{
			_hasReachedEnd = false;

			_ctxStack.Clear();
			_context = new WriterContext();
			_ctxStack.Push(_context);

			if (_instStringBuilder != null)
			{
				_instStringBuilder.Remove(0, _instStringBuilder.Length);
			}
		}

		/// <summary>
		///     Writes the specified boolean.
		/// </summary>
		/// <param name="boolean">if set to <c>true</c> [boolean].</param>
		public void Write(bool boolean)
		{
			DoValidation(Condition.Value);
			PutNewline();
			Put(boolean ? "true" : "false");

			_context.ExpectingValue = false;
		}

		/// <summary>
		///     Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write(decimal number)
		{
			DoValidation(Condition.Value);
			PutNewline();
			Put(Convert.ToString(number, NumberFormat));

			_context.ExpectingValue = false;
		}

		/// <summary>
		///     Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write(double number)
		{
			DoValidation(Condition.Value);
			PutNewline();

			var str = Convert.ToString(number, NumberFormat);
			Put(str);

			if (str.IndexOf('.') == -1 && str.IndexOf('E') == -1)
			{
				TextWriter.Write(".0");
			}

			_context.ExpectingValue = false;
		}

		/// <summary>
		///     Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write(float number)
		{
			DoValidation(Condition.Value);
			PutNewline();

			var str = Convert.ToString(number, NumberFormat);
			Put(str);

			if (str.IndexOf('.') == -1 && str.IndexOf('E') == -1)
			{
				TextWriter.Write(".0");
			}

			_context.ExpectingValue = false;
		}

		/// <summary>
		///     Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write(int number)
		{
			DoValidation(Condition.Value);
			PutNewline();
			Put(Convert.ToString(number, NumberFormat));

			_context.ExpectingValue = false;
		}

		/// <summary>
		///     Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write(long number)
		{
			DoValidation(Condition.Value);
			PutNewline();
			Put(Convert.ToString(number, NumberFormat));

			_context.ExpectingValue = false;
		}

		/// <summary>
		///     Writes the specified STR.
		/// </summary>
		/// <param name="str">The STR.</param>
		public void Write(string str)
		{
			DoValidation(Condition.Value);
			PutNewline();

			if (str == null)
			{
				Put("null");
			}
			else
			{
				PutString(str);
			}

			_context.ExpectingValue = false;
		}

		/// <summary>
		///     Writes the specified rawValue without any changes. May corrupt json!
		/// </summary>
		/// <param name="str">The STR.</param>
		public void WriteRawValue(string rawValue)
		{
			DoValidation(Condition.Value);
			PutNewline();
			Put(rawValue);

			_context.ExpectingValue = false;
		}

		/// <summary>
		///     Writes the specified number.
		/// </summary>
		/// <param name="number">The number.</param>
		public void Write(ulong number)
		{
			DoValidation(Condition.Value);
			PutNewline();

			Put(Convert.ToString(number, NumberFormat));

			_context.ExpectingValue = false;
		}

		/// <summary>
		///     Writes the array end.
		/// </summary>
		public void WriteArrayEnd()
		{
			DoValidation(Condition.InArray);
			PutNewline(false);

			_ctxStack.Pop();
			if (_ctxStack.Count == 1)
			{
				_hasReachedEnd = true;
			}
			else
			{
				_context = _ctxStack.Peek();
				_context.ExpectingValue = false;
			}

			Unindent();
			Put("]");
		}

		/// <summary>
		///     Writes the array start.
		/// </summary>
		public void WriteArrayStart()
		{
			DoValidation(Condition.NotAProperty);
			PutNewline();

			Put("[");

			_context = new WriterContext {InArray = true};
			_ctxStack.Push(_context);

			Indent();
		}

		/// <summary>
		///     Writes the object end.
		/// </summary>
		public void WriteObjectEnd()
		{
			DoValidation(Condition.InObject);
			PutNewline(false);

			_ctxStack.Pop();
			if (_ctxStack.Count == 1)
			{
				_hasReachedEnd = true;
			}
			else
			{
				_context = _ctxStack.Peek();
				_context.ExpectingValue = false;
			}

			Unindent();
			Put("}");
		}

		/// <summary>
		///     Writes the object start.
		/// </summary>
		public void WriteObjectStart()
		{
			DoValidation(Condition.NotAProperty);
			PutNewline();

			Put("{");

			_context = new WriterContext {InObject = true};
			_ctxStack.Push(_context);

			Indent();
		}

		/// <summary>
		///     Writes the name of the property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		public void WritePropertyName(string propertyName)
		{
			DoValidation(Condition.Property);
			PutNewline();

			PutString(propertyName);

			if (PrettyPrint)
			{
				if (propertyName.Length > _context.Padding)
				{
					_context.Padding = propertyName.Length;
				}

				for (var i = _context.Padding - propertyName.Length; i >= 0; i--)
				{
					TextWriter.Write(' ');
				}

				TextWriter.Write(": ");
			}
			else
			{
				TextWriter.Write(':');
			}
			_context.ExpectingValue = true;
		}
	}
}
