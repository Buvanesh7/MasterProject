using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AkryazTools.Converters
{
    public class StringToIntegerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is byte byteValue)) return Binding.DoNothing;

            return byteValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string stringValue)) return Binding.DoNothing;

            return byte.TryParse(stringValue, out var result) ? result : Binding.DoNothing;
        }
    }
}
