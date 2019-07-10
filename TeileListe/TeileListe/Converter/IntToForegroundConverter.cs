using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TeileListe.Converter
{
    class IntToForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int && (int) value < 0)
            {
                return new BrushConverter().ConvertFrom("#17d117");
            }
            if (value is int && (int)value > 0)
            {
                return new BrushConverter().ConvertFrom("#ff0000");
            }
            return new BrushConverter().ConvertFrom("#000000");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
