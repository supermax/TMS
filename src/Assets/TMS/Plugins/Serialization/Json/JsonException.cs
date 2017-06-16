#region

using System;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	/// </summary>
	public class JsonException : Exception
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="JsonException" /> class.
		/// </summary>
		public JsonException()
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonException" /> class.
		/// </summary>
		/// <param name="token">The token.</param>
		internal JsonException(ParserToken token) :
			base(string.Format(
				"Invalid token '{0}' in input string", token))
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonException" /> class.
		/// </summary>
		/// <param name="token">The token.</param>
		/// <param name="innerException">The inner exception.</param>
		internal JsonException(ParserToken token,
			Exception innerException) :
				base(string.Format(
					"Invalid token '{0}' in input string", token),
					innerException)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonException" /> class.
		/// </summary>
		/// <param name="c">The c.</param>
		internal JsonException(int c) :
			base(string.Format(
				"Invalid character '{0}' in input string", (char) c))
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonException" /> class.
		/// </summary>
		/// <param name="c">The c.</param>
		/// <param name="innerException">The inner exception.</param>
		internal JsonException(int c, Exception innerException) :
			base(string.Format(
				"Invalid character '{0}' in input string", (char) c),
				innerException)
		{
		}


		/// <summary>
		///     Initializes a new instance of the <see cref="JsonException" /> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public JsonException(string message) : base(message)
		{
		}

		/// <summary>
		///     Initializes a new instance of the <see cref="JsonException" /> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">
		///     The exception that is the cause of the current exception, or a null reference (Nothing in
		///     Visual Basic) if no inner exception is specified.
		/// </param>
		public JsonException(string message, Exception innerException) :
			base(message, innerException)
		{
		}
	}
}