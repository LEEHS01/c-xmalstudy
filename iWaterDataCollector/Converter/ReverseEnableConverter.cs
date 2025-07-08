using System;
using System.Globalization;
using System.Windows.Data;

namespace iWaterDataCollector.Converter
{
    /// <summary>
    /// Enable Property Converter - Reverse for use with <see cref="IValueConverter"/>
    /// </summary>
    /// <remarks>
    /// Binding Data에 따른 Control 활성화/비활성화 반전 Converter
    /// </remarks>
    public class ReverseEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isEnable = System.Convert.ToBoolean(value);
            return !isEnable;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
