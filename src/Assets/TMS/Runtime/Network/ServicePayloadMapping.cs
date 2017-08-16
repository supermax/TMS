#region

using System;

#endregion

namespace TMS.Common.Network
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

		public override object TypeId
		{
			get { return GetHashCode(); }
		}

		public virtual bool IsOneWay { get; set; }
		public virtual Type ServiceResponsePayloadType { get; set; }

		public override bool Match(object obj)
		{
			var equals = Equals(this, obj);
			return equals;
		}

		public override bool IsDefaultAttribute()
		{
			return true;
		}
	}
}