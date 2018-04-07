using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace TeileListe.Converter
{
    class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolVal = value as bool?;
            if (boolVal.HasValue && boolVal.Value)
            {
                return new BrushConverter().ConvertFrom("#17d117");
            }
            return new BrushConverter().ConvertFrom("#ffb400");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
