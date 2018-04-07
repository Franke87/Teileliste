using System;
using System.Globalization;
using System.Windows.Data;

namespace TeileListe.Converter
{
    public class DecimalToWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = (decimal)value;

            return intValue.ToString("N1") + " g";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = (string)value;
            stringValue = stringValue.Replace("kg", "");
            stringValue = stringValue.Replace("g", "");
            stringValue = stringValue.Replace(".", "");
            stringValue = stringValue.Replace(" ", "");
            decimal retValue;
            if (decimal.TryParse(stringValue, NumberStyles.AllowDecimalPoint, new CultureInfo("de-DE"), out retValue))
            {
                if (retValue < 0)
                {
                    retValue = 0;
                }
                return retValue;
            }
            return 0;
        }
    }
}
