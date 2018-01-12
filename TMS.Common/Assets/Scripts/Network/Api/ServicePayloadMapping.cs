#region

using System;

#endregion

namespace TMS.Common.Network.Api
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ServicePayloadMapping : Attribute
	{
		public ServicePayloadMapping(bool isOneWay = true)
		{
			IsOneWay = isOneWay;
		}

		public ServicePayloadMapping(Type serviceResponsePayloadType)
		{
			ServiceResponsePayloadType = serviceResponsePayloadType;
		}

		public virtual bool IsOneWay { get; set; }

		public virtual Type ServiceResponsePayloadType { get; set; }

#if !UNITY_WSA
		public override object TypeId
		{
			get { return GetHashCode(); }
		}

		public override bool Match(object obj)
		{
			var equals = Equals(this, obj);
			return equals;
		}

		public override bool IsDefaultAttribute()
		{
			return true;
		}
#endif
	}
}