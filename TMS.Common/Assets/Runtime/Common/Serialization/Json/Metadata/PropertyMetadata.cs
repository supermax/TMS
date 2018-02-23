#region

using System;
using System.Reflection;
using TMS.Common.Extensions;

#endregion

namespace TMS.Common.Serialization.Json
{
	/// <summary>
	/// Property Metadata
	/// </summary>
	internal class PropertyMetadata
	{
		/// <summary>
		///     The info
		/// </summary>	
		public MemberInfo Info 
		{ 
			get; 
			private set; 
		}

		/// <summary>
		///     The is field
		/// </summary>
		public bool IsField 
		{ 
			get; 
			private set; 
		}

		/// <summary>
		///     The type
		/// </summary>
		public Type Type 
		{
			get;
			private set; 
		}

		/// <summary>
		/// Gets or sets the attribute.
		/// </summary>
		/// <value>
		/// The attribute.
		/// </value>
		public JsonDataMemberAttribute Attribute 
		{ 
			get; 
			private set; 
		}

		public string MemberName
		{
			get
			{
				var memberName = Attribute != null && !Attribute.Name.IsNullOrEmpty() ? Attribute.Name : Info.Name;
				return memberName;
			}
		}

		internal PropertyMetadata(Type type, MemberInfo info, bool isField, JsonDataMemberAttribute attribute)
		{
			Type = type;
			Info = info;
			IsField = isField;
			Attribute = attribute;
		}

		/// <summary>
		/// Gets the type of the member.
		/// </summary>
		/// <returns></returns>
		public Type GetMemberType()
		{
			Type res;
			if (IsField)
			{
				var fieldInfo = (FieldInfo) Info;
				res = fieldInfo.FieldType;
			}
			else
			{
				var propInfo = (PropertyInfo) Info;
				res = propInfo.PropertyType;
			}
			return res;
		}

	    public override string ToString()
	    {
	        return string.Format("Type: {0}, Info: {1}, IsField: {2}", GetMemberType(), Info, IsField);
	    }
	}
}