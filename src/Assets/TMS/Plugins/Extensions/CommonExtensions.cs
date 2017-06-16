#region

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
#if !NETFX_CORE

#else
#endif
using TMS.Common.Serialization.Json;
#endregion

namespace TMS.Common.Extensions
{
	/// <summary>
	///     Common Extensions
	/// </summary>
	public static class CommonExtensions
	{
		/// <summary>
		/// Initializes variable the with lock.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="locker">The locker.</param>
		/// <param name="value">The value.</param>
		/// <param name="initFunc">The initialization function.</param>
		/// <returns></returns>
		public static T InitWithLock<T>(this object locker, ref T value, Func<T> initFunc)
		{
			if (Equals(value, default(T)))
			{
				lock (locker)
				{
					if (Equals(value, default(T)))
					{
						value = initFunc();
					}
				}
			}
			return value;
		}

		/// <summary>
		///     Determines whether [is null or empty] [the specified string].
		/// </summary>
		/// <param name="str">The STR.</param>
		/// <returns>
		///     <c>true</c> if [is null or empty] [the specified string]; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullOrEmpty(this string str)
		{
			return String.IsNullOrEmpty(str);
		}

		/// <summary>
		///     Gets the name of the member.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>System.String.</returns>
		/// <example>
		/// var anon = new {FirstName = "My Name"};
		/// var propName = anon.GetMemberName(() => anon.FirstName);
		/// </example>
		public static string GetMemberName(this object sender, LambdaExpression expression)
		{
			var memberExpression = (MemberExpression) expression.Body;
			return memberExpression.Member.Name;
		}

		/// <summary>
		///     Gets the display name.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>System.String.</returns>
		public static string GetDisplayName(this object sender, LambdaExpression expression)
		{
			var memberExpression = (MemberExpression) expression.Body;
			var name = memberExpression.GetDisplayName();
			return name;
		}

		/// <summary>
		///     Gets the display name.
		/// </summary>
		/// <param name="memberExpression">The member expression.</param>
		/// <returns>System.String.</returns>
		public static string GetDisplayName(this MemberExpression memberExpression)
		{
#if !UNITY3D && !SILVERLIGHT && !NETFX_CORE && !UNITY_WP8
			var attribs = memberExpression.Member.GetCustomAttributes(typeof (DisplayNameAttribute), true);
			if (!attribs.IsNullOrEmpty())
			{
				var attrib = attribs.GetFirstOrDefault() as DisplayNameAttribute;
				if (attrib != null)
				{
					return attrib.DisplayName;
				}
			}
#endif
			return memberExpression.Member.Name;
		}

		/// <summary>
		///     Gets the name of the member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sender">The sender.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>System.String.</returns>
		public static string GetMemberName<T>(this object sender, Expression<Func<T>> expression)
		{
			var memberExpression = (MemberExpression) expression.Body;
			return memberExpression.Member.Name;
		}

		/// <summary>
		///     Gets the display name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sender">The sender.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>System.String.</returns>
		public static string GetDisplayName<T>(this object sender, Expression<Func<T>> expression)
		{
			var memberExpression = (MemberExpression) expression.Body;
			var name = memberExpression.GetDisplayName();
			return name;
		}

		/// <summary>
		///     Gets the name of the member.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sender">The sender.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>System.String.</returns>
		public static string GetMemberName<T>(this object sender, Expression<Action<T>> expression)
		{
			var memberExpression = (MemberExpression) expression.Body;
			return memberExpression.Member.Name;
		}

		/// <summary>
		///     Gets the display name.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sender">The sender.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>System.String.</returns>
		public static string GetDisplayName<T>(this object sender, Expression<Action<T>> expression)
		{
			var memberExpression = (MemberExpression) expression.Body;
			var name = memberExpression.GetDisplayName();
			return name;
		}

		/// <summary>
		///     Gets the name of the member.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>System.String.</returns>
		public static string GetMemberName(this object sender, Expression<Action> expression)
		{
			var memberExpression = (MemberExpression) expression.Body;
			return memberExpression.Member.Name;
		}

		/// <summary>
		///     Gets the display name.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="expression">The expression.</param>
		/// <returns>System.String.</returns>
		public static string GetDisplayName(this object sender, Expression<Action> expression)
		{
			var memberExpression = (MemberExpression) expression.Body;
			var name = memberExpression.GetDisplayName();
			return name;
		}

		/// <summary>
		///     Converts to culture invariant string.
		/// </summary>
		/// <param name="num">The num.</param>
		/// <returns></returns>
		public static string AsString(this int num)
		{
			return num.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts to culture invariant string.
		/// </summary>
		/// <param name="num">The num.</param>
		/// <returns></returns>
		public static string AsString(this int? num)
		{
			return num.HasValue ? num.Value.AsString() : null;
		}

		/// <summary>
		///     Converts to culture invariant string.
		/// </summary>
		/// <param name="ch">The char.</param>
		/// <returns></returns>
		public static string AsString(this char ch)
		{
#if !NETFX_CORE
			return ch.ToString(CultureInfo.InvariantCulture);
#else
			return ch.ToString();
#endif
		}

		/// <summary>
		///     Converts to culture invariant string.
		/// </summary>
		/// <param name="num">The num.</param>
		/// <returns></returns>
		public static string AsString(this double num)
		{
			return num.ToString(CultureInfo.InvariantCulture);
		}

		/// <summary>
		///     Converts to culture invariant string.
		/// </summary>
		/// <param name="num">The num.</param>
		/// <returns></returns>
		public static string AsString(this double? num)
		{
			return num.HasValue ? num.Value.AsString() : null;
		}

		/// <summary>
		/// Determines whether the specified double (a) is equals to other double (b)
		/// </summary>
		/// <param name="a">The source.</param>
		/// <param name="b">The target.</param>
		/// <returns></returns>
		public static bool IsEquals(this double a, double b)
		{
			var res = a.Equals(b);
			return res;
		}

		/// <summary>
		/// Determines whether given value is not a number.
		/// </summary>
		/// <param name="d">The command.</param>
		/// <returns></returns>
		public static bool IsNaN(this double d)
		{
			return Double.IsNaN(d);
		}
	}
}