#region Code Editor
// Maxim
#endregion

using System;
using System.Collections.Generic;
using TMS.Common.Serialization.Json;

namespace TMS.Common.Core
{
	/// <summary>
	/// Base Network Event Payload
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BaseNetworkEventPayload<T> : EventArgs
	{
		/// <summary>
		/// Gets or sets the response arguments.
		/// </summary>
		/// <value>
		/// The response arguments.
		/// </value>
		public virtual IDictionary<string, object> ResponseArgs { get; set; }

		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>
		/// The response.
		/// </value>
		public virtual T Response { get; set; }

		/// <summary>
		/// Gets or sets the error.
		/// </summary>
		/// <value>
		/// The error.
		/// </value>
		public virtual string Error { get; set; }

		/// <summary>
		/// Gets or sets the exception.
		/// </summary>
		/// <value>
		/// The exception.
		/// </value>
		public virtual Exception Exception { get; set; }

		/// <summary>
		/// Gets or sets the event sender.
		/// </summary>
		/// <value>
		/// The event sender.
		/// </value>
		public virtual object EventSender { get; protected internal set; }

		/// <summary>
		/// Gets or sets the type of the event.
		/// </summary>
		/// <value>
		/// The type of the event.
		/// </value>
		public virtual string EventType { get; protected internal set; }

		/// <summary>
		/// Gets or sets a value indicating whether [handled].
		/// </summary>
		/// <value>
		///   <c>true</c> if [handled]; otherwise, <c>false</c>.
		/// </value>
		public virtual bool Handled { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseNetworkEventPayload{T}"/> class.
		/// </summary>
		protected BaseNetworkEventPayload()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseNetworkEventPayload{T}"/> class.
		/// </summary>
		/// <param name="eventType">Type of the event.</param>
		/// <param name="response">The response.</param>
		/// <param name="error">The error.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="responseArgs">The response arguments.</param>
		protected BaseNetworkEventPayload(string eventType, T response, string error, Exception exception = null, IDictionary<string, object> responseArgs = null)
		{
			EventType = eventType;
			Response = response;
			Error = error;
			Exception = exception;
			ResponseArgs = responseArgs;
		}
	}

	/// <summary>
	/// Network Event Payload
	/// </summary>
	public abstract class NetworkEventPayload : BaseNetworkEventPayload<JsonData>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkEventPayload"/> class.
		/// </summary>
		protected NetworkEventPayload()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NetworkEventPayload"/> class.
		/// </summary>
		/// <param name="eventType">Type of the event.</param>
		/// <param name="response">The response.</param>
		/// <param name="error">The error.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="responseArgs">The response arguments.</param>
		protected NetworkEventPayload(string eventType, JsonData response, string error, 
										Exception exception = null, IDictionary<string, object> responseArgs = null) : 
										base(eventType, response, error, exception, responseArgs)
		{
			
		}
	}

	/// <summary>
	/// Base Request
	/// </summary>
	public abstract class BaseRequest
	{
		/// <summary>
		/// Gets or sets the servlet.
		/// </summary>
		/// <value>
		/// The servlet.
		/// </value>
		public string Servlet { get; set; }

		/// <summary>
		/// Gets or sets the command.
		/// </summary>
		/// <value>
		/// The command.
		/// </value>
		public string Cmd { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		public string Uid { get; set; }

		/// <summary>
		/// Gets or sets the SID.
		/// </summary>
		/// <value>
		/// The SID.
		/// </value>
		public string Sid { get; set; }

		private readonly IDictionary<string, string> _vars = new Dictionary<string, string>();

		/// <summary>
		/// Gets the vars.
		/// </summary>
		/// <value>
		/// The vars.
		/// </value>
		public IDictionary<string, string> Vars { get { return _vars; } }
	}
}