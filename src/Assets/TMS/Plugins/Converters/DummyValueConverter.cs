#region Usings

using System;
using System.Diagnostics;
using System.Globalization;
using TMS.Common.Helpers;
using UnityEngine;

#endregion

namespace TMS.Common.Converters
{
	/// <summary>
	///     Dummy Value Converter (for binding debug)
	/// </summary>
	public class DummyValueConverter : IValueConverter
	{
		/// <summary>
		/// Gets or sets a value indicating whether this instance is break enabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is break enabled; otherwise, <c>false</c>.
		/// </value>
		public bool IsBreakEnabled { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="DummyValueConverter"/> class.
		/// </summary>
		public DummyValueConverter()
		{
			IsBreakEnabled = true;
		}

		#region IValueConverter Members

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
#if DEBUG
			if (!IsBreakEnabled || Application.isEditor || !Debugger.IsAttached) return value;
			Debugger.Break();
			return value;
#else
			return value;
#endif
		}

		/// <summary>
		///     Converts a value.
		/// </summary>
		/// <param name="value">The value that is produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value. If the method returns null, the valid null value is used.</returns>
		public object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
			CultureInfo culture
#else
 string language
#endif
			)
		{
#if DEBUG
			if (IsBreakEnabled || Application.isEditor || !Debugger.IsAttached) return value;
			Debugger.Break();
			return value;
#else
			return value;
#endif
		}

		#endregion

#if !NETFX_CORE && !SILVERLIGHT
		/// <summary>
		///     When implemented in a derived class, returns an object that is provided as the value of the target property for this markup extension.
		/// </summary>
		/// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
		/// <returns>The object value to set on the property where the extension is applied.</returns>
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
#endif
	}
}