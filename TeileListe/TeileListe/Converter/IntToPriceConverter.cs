using System;
using System.Globalization;
using System.Windows.Data;

namespace TeileListe.Converter
{
    class IntToPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = (int)value;
            intValue = intValue < 0 ? 0 : intValue;
            var retVal = String.Format(new CultureInfo("de-DE"), "{0:C2}", (float)intValue / 100);

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal decimalValue;
            if (decimal.TryParse((string)value, NumberStyles.Currency, new CultureInfo("de-DE"), out decimalValue))
            {
                decimalValue = decimalValue < 0 ? 0 : decimalValue;
                return (int) (decimalValue*100);
            }
            return 0;
        }
    }
}
