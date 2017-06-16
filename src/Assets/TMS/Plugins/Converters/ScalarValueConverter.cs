#region

using System;
using System.Globalization;

#endregion

namespace TMS.Common.Converters
{
	/// <summary>
	/// Scalar Value Converter
	/// </summary>
	public class ScalarValueConverter : IValueConverter
	{
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
			if (value == null || parameter == null)
			{
				return null;
			}

			var original = (double)value;
			double scalar;
			if (!double.TryParse(parameter.ToString(), out scalar)) return original;

			var result = original * scalar;
			return result;
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
		public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
 CultureInfo culture
#else
 string language
#endif
)
		{
			return Convert(value, targetType, parameter, null);
		}

		#endregion
	}
}