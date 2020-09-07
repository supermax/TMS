using System.Reflection;
using TMS.Common.Extensions;
using TMS.Common.Serialization.Json.Api;

namespace TMS.Common.Serialization.Json.Helpers
{
    internal static class ReflectionHelper
    {
        /// <summary>
		/// Gets the data member attribute.
		/// </summary>
		/// <param name="info">The information.</param>
		/// <returns></returns>
		internal static JsonDataMemberAttribute GetDataMemberAttribute(MemberInfo info)
		{
			var attributes = info.GetCustomAttributes(typeof (JsonDataMemberAttribute), true);
			var attribute = attributes.GetFirstOrDefault() as JsonDataMemberAttribute;
			return attribute ;
		}

		/// <summary>
		///     Determines whether [is ignorable member] [by the specified member information].
		/// </summary>
		/// <param name="info">The member information.</param>
		/// <returns></returns>
		internal static bool IsIgnorableMember(MemberInfo info)
		{
			var attributes = info.GetCustomAttributes(typeof (JsonDataMemberIgnoreAttribute), true);
			var isIgnorable = !attributes.IsNullOrEmpty();
			return isIgnorable;
		}
    }
}