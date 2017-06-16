using System;
using System.Globalization;

namespace TMS.Common.Converters
{
	public interface IValueConverter
	{
		object Convert(object value, Type targetType, object parameter,
#if !NETFX_CORE
			CultureInfo culture
#else
			string language
#endif
		);

		object ConvertBack(object value, Type targetType, object parameter,
#if !NETFX_CORE
			CultureInfo culture
#else
			string language
#endif
		);
	}
}