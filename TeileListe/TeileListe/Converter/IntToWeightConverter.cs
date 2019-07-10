using System;
using System.Globalization;
using System.Windows.Data;

namespace TeileListe.Converter
{
    class IntToWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = (int)value;
            var prefix = string.Empty;
            if (parameter is string && (string)parameter == "1" && intValue > 0)
            {
                prefix = "+";
            }

            if (intValue >= 1000)
            {
                var retValue = (decimal)intValue / 1000;
                return prefix + retValue.ToString("N3") + " kg";
            }
            return prefix + intValue.ToString("N0") + " g";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = (string)value;
            if (stringValue.Contains("kg") || stringValue.IndexOf(',') >= 0)
            {
                stringValue = stringValue.Replace("kg", "");
                stringValue = stringValue.Replace("g", "");
                stringValue = stringValue.Replace(" ", "");
                stringValue = stringValue.Replace(".", "");
                decimal decimalValue;
                if (decimal.TryParse(stringValue, NumberStyles.AllowDecimalPoint, new CultureInfo("de-DE"), out decimalValue))
                {
                    return (int)(decimalValue * 1000);
                }
            }
            else
            {
                stringValue = stringValue.Replace("g", "");
                int retValue;
                if (int.TryParse(stringValue, NumberStyles.Number, new CultureInfo("de-DE"), out retValue))
                {
                    if (retValue < 0)
                    {
                        retValue = 0;
                    }
                    return retValue;
                }
            }
            return 0;
        }
    }
}
