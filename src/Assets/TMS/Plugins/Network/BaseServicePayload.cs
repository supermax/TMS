using TMS.Common.Serialization.Json;

namespace TMS.Common.Network
{
	/// <summary>
	/// Base Service Payload
	/// </summary>
	[JsonDataContract]
	public abstract class BaseServicePayload
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseServicePayload"/> class.
		/// </summary>
		/// <param name="serviceName">Name of the service.</param>
		protected BaseServicePayload(string serviceName)
		{
			ServiceName = serviceName;
		}

		/// <summary>
		/// Gets or sets the name of the service.
		/// </summary>
		/// <value>
		/// The name of the service.
		/// </value>
		[JsonDataMember]
		public virtual string ServiceName { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is custom service and not registered in config file.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is custom service; otherwise, <c>false</c>.
		/// </value>
		[JsonDataMember]
		public virtual bool IsCustomService { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance is handled.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is handled; otherwise, <c>false</c>.
		/// </value>
		[JsonDataMemberIgnore]
		public virtual bool IsHandled { get; set; }
	}
}