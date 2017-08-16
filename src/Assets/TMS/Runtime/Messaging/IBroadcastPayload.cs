using System;

namespace TMS.Common.Messaging
{
	/// <summary>
	/// Interface for Messenger Payloads
	/// </summary>
	public interface IMessengerPayload
	{
		/// <summary>
		/// Gets or sets a value indicating whether this payload is handled.
		/// </summary>
		/// <value>
		///   <c>true</c> if is handled; otherwise, <c>false</c>.
		/// </value>
		bool IsHandled { get; set; }
	}

	/// <summary>
	/// Interface for BroadcastPayload
	/// </summary>
	public interface IBroadcastPayload : IMessengerPayload
	{
		/// <summary>
		/// Gets or sets the info.
		/// </summary>
		/// <value>
		/// The info.
		/// </value>
		IBroadcastPayloadInfo Info { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is trace enabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is trace enabled; otherwise, <c>false</c>.
		/// </value>
		bool IsTraceEnabled { get; set; }
	}

	/// <summary>
	/// Interface for BroadcastPayloadInfo
	/// </summary>
	public interface IBroadcastPayloadInfo
	{
		/// <summary>
		/// Gets the publish start time.
		/// </summary>
		/// <value>
		/// The publish start time.
		/// </value>
		DateTime PublishStartTime { get; }

		/// <summary>
		/// Gets the publish end time.
		/// </summary>
		/// <value>
		/// The publish end time.
		/// </value>
		DateTime PublishEndTime { get; }
	}

	/// <summary>
	/// Broadcast Payload Info
	/// </summary>
	internal class BroadcastPayloadInfo : IBroadcastPayloadInfo
	{
		/// <summary>
		/// Gets the publish start time.
		/// </summary>
		/// <value>
		/// The publish start time.
		/// </value>
		public DateTime PublishStartTime { get; set; }

		/// <summary>
		/// Gets the publish end time.
		/// </summary>
		/// <value>
		/// The publish end time.
		/// </value>
		public DateTime PublishEndTime { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			return string.Format("[{0}] PublishStartTime: {1}, PublishEndTime: {2}", GetType(), PublishStartTime, PublishEndTime);
		}
	}
}