#region

using System;
using System.Globalization;

#endregion

namespace TMS.Common.Converters
{
	/// <summary>
	/// Boolean To Opacity Converter
	/// </summary>
	public class BooleanToOpacityConverter : IValueConverter
	{
		/// <summary>
		/// Gets or sets the true opacity value.
		/// </summary>
		/// <value>
		/// The true opacity value.
		/// </value>
		public double TrueOpacityValue { get; set; }

		/// <summary>
		/// The default true opacity value
		/// </summary>
		public const double DefaultTrueOpacityValue = 1.0;

		/// <summary>
		/// Gets or sets the false opacity value.
		/// </summary>
		/// <value>
		/// The false opacity value.
		/// </value>
		public double FalseOpacityValue { get; set; }

		/// <summary>
		/// The default false opacity value
		/// </summary>
		public const double DefaultFalseOpacityValue = 0.5;

		/// <summary>
		/// Initializes a new instance of the <see cref="BooleanToOpacityConverter"/> class.
		/// </summary>
		public BooleanToOpacityConverter()
		{
			TrueOpacityValue = DefaultTrueOpacityValue;
			FalseOpacityValue = DefaultFalseOpacityValue;
		}

		#region Implementation of IValueConverter

		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		/// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
		public object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
 CultureInfo culture
#else
 string language
#endif
)
		{
			if (value == null || !(value is bool)) return TrueOpacityValue;
			return (bool) value ? TrueOpacityValue : FalseOpacityValue;
		}

		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		/// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
		public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
 CultureInfo culture
#else
 string language
#endif
)
		{
			return null;
		}

		#endregion
	}
}