#region Usings

using System;
using TMS.Common.Extensions;
using System.Globalization;

#endregion

namespace TMS.Common.Converters
{
	/// <summary>
	///     Class StringFormatConverter
	/// </summary>
	public class StringFormatConverter : IValueConverter
	{
		/// <summary>
		///     Converts a value.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		public object Convert(object value, Type targetType, object parameter, 
#if !NETFX_CORE
			CultureInfo culture
#else
			string language
#endif
			)
		{
			ArgumentValidator.AssertNotNull(value, "value");
			ArgumentValidator.AssertNotNull(parameter, "parameter");
			
			var str = string.Format(parameter.ToString(), value);
			return str;
		}

		/// <summary>
		///     Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
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
	}
}