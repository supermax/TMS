#region Usings

using System;
using TMS.Common.Network.Api;
using TMS.Common.Network.Request.Api;
using TMS.Common.Network.Response.Api;
using TMS.Common.Serialization.Json;
using TMS.Common.Serialization.Json.Api;

#endregion

namespace TMS.Common.Network.Response.Api
{
	/// <summary>
	///     Service Response PayloadHash
	/// </summary>
	/// <typeparam name="T">Data Type</typeparam>
	public class ServiceResponsePayload<T> : BaseServiceResponsePayload, IServiceResponsePayload<T>
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="ServiceResponsePayload{T}" /> class.
		/// </summary>
		/// <param name="parentRequestPayload">The parent request payload.</param>
		/// <param name="data">The data.</param>
		/// <param name="state">The state.</param>
		/// <param name="error">The error.</param>
		public ServiceResponsePayload(IServiceRequestPayload parentRequestPayload,
			T data, ServiceResponseState state, Exception error = null) :
			base(parentRequestPayload, state, error)
		{
			ResponseData = data;
		}

		[JsonDataMember("data")]
		public virtual T ResponseData { get; set; }
	}
}