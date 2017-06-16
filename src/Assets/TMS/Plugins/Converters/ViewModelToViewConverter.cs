using System;
using System.Globalization;
#if !NETFX_CORE
#else
using Windows.UI.Xaml.Data;
#endif

using TMS.Common.Modularity.Regions.Adapters;

namespace TMS.Common.Converters
{
	/// <summary>
	/// View Model to View Converter
	/// </summary>
	public class ViewModelToViewConverter : IValueConverter
	{
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
			if (value == null) return null;
			var view = RegionAdapterHelper.CreateView(value);
			return view;
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
			throw new NotImplementedException();
		}

		#endregion
	}
}