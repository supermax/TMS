#region Usings

using TMS.Common.Serialization.Json;
using System.Collections.Generic;
#if UNITY3D || UNITY_3D
using UnityEngine;

#endif

#endregion

namespace TMS.Common.Network
{
	[JsonDataContract]
	public abstract class BaseServiceRequestPayload : BaseServicePayload
	{
		protected BaseServiceRequestPayload(string serviceName) : base(serviceName)
		{
		}

		[JsonDataMember]
		public virtual string CustomServiceUrl { get; set; }

		[JsonDataMember]
		public virtual Dictionary<string, string> UrlParams { get; set; }

		[JsonDataMember]
		public virtual Dictionary<string, string> PostParams { get; set; }

		[JsonDataMember]
		public virtual Dictionary<string, string> HeaderParams { get; set; }

		[JsonDataMemberIgnore]
		public virtual bool Abort { get; set; }

		[JsonDataMemberIgnore]
		public virtual bool AbortAll { get; set; }

		[JsonDataMemberIgnore]
		public string ResponseCallbackId { get; set; }

		[JsonDataMember]
		public virtual byte[] RawRequestData { get; set; }

		[JsonDataMember]
		public virtual ResponsePayloadDataType ResponseDataType { get; set; } // TODO try to discover data type from <T> (response data type)

		[JsonDataMember]
		public virtual ServiceRequestType RequestType { get; set; }

		[JsonDataMember]
		public virtual NetworkRequestConfiguration Config { get; set; }

		public virtual BaseServiceResponsePayload CreateServiceResponsePayload(string responseString,
			ServiceResponseState state, string error = null)
		{
			return null;
		}		

#if UNITY3D || UNITY_3D
		public virtual BaseServiceResponsePayload CreateServiceResponsePayload(Texture2D image,
			ServiceResponseState state, string error = null)
		{
			return null;
		}
#endif
	}

	[JsonDataContract]
	public abstract class BaseServiceRequestPayload<T> : BaseServiceRequestPayload
	{
		protected BaseServiceRequestPayload(string serviceName, T data) : base(serviceName)
		{
			RequestData = data;
		}

		[JsonDataMember]
		public virtual T RequestData { get; set; }

		//public virtual BaseServiceResponsePayload<T> CreateServiceResponsePayload(string responseString,
		//	ServiceResponseState state, string error = null)
		//{
		//	return null;
		//}
	}

	/// <summary>
	/// Response Payload Data Type
	/// </summary>
	public enum ResponsePayloadDataType
	{
		/// <summary>
		/// string
		/// </summary>
		String = 0,

		/// <summary>
		/// image
		/// </summary>
		Image,

		/// <summary>
		/// sound
		/// </summary>
		Sound,

		/// <summary>
		/// byte array
		/// </summary>
		ByteArray,

		/// <summary>
		/// other
		/// </summary>
		Other
	}

	/// <summary>
	/// Service Request Type
	/// </summary>
	public enum ServiceRequestType
	{
		/// <summary>
		/// POST
		/// </summary>
		Post = 0,

		/// <summary>
		/// GET
		/// </summary>
		Get
	}
}