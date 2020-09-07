#define UNITY3D

#region Usings

using System;

#endregion

namespace TMS.Common.Serialization.Json.Api
{
	/// <summary>
	///     JSON Data Member Ignore Attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class JsonDataMemberIgnoreAttribute : Attribute
	{
		
	}
}