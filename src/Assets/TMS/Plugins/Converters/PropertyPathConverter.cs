#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Globalization;

#endregion

namespace TMS.Common.Converters
{
	/// <summary>
	/// Property Path Converter
	/// </summary>
	public class PropertyPathConverter : IValueConverter
	{
		private static readonly IDictionary<Type, IDictionary<string, PropertyInfo>> TypePropsDic =
			new Dictionary<Type, IDictionary<string, PropertyInfo>>();

		/// <summary>
		/// Gets the prop info.
		/// </summary>
		/// <param name="srcType">Type of the SRC.</param>
		/// <param name="propName">Name of the prop.</param>
		/// <returns></returns>
		private static PropertyInfo GetPropInfo(Type srcType, string propName)
		{
			IDictionary<string, PropertyInfo> propsDic;
			if (TypePropsDic.ContainsKey(srcType))
			{
				propsDic = TypePropsDic[srcType];
			}
			else
			{
				propsDic = new Dictionary<string, PropertyInfo>();
				TypePropsDic[srcType] = propsDic;
			}

			PropertyInfo propInfo;
			if (propsDic.ContainsKey(propName))
			{
				propInfo = propsDic[propName];
			}
			else
			{
				propInfo = srcType.GetProperty(propName);
				propsDic[propName] = propInfo;
			}
			return propInfo;
		}

		#region Implementation of IValueConverter

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
			CultureInfo culture
#else
			string language
#endif
			)
		{
			if (value == null)
			{
				Debug.WriteLine("[{0}]: Value is NULL or Unset", typeof (PropertyPathConverter).Name);
				return null;
			}
			if (parameter == null)
			{
				Debug.WriteLine("[{0}]: Parameter is NULL", typeof (PropertyPathConverter).Name);
				return null;
			}

			var propInfo = GetPropInfo(value.GetType(), parameter.ToString());
			var res = propInfo.GetValue(value, null);
			return res;
		}

		/// <summary>
		/// Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
			CultureInfo culture
#else
			string language
#endif
			)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}