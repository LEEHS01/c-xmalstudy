using System;
using System.Globalization;
using System.Windows.Data;

namespace iWaterDataCollector.Converter
{
    /// <summary>
    /// Visibility Property Converter - Reverse for use with <see cref="IValueConverter"/>
    /// </summary>
    /// <remarks>
    /// Binding Data에 따른 Control Visibility(Collapsed / Visible) 반전 Converter
    /// </remarks>
    public class ReverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = System.Convert.ToBoolean(value);
            return isVisible ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
