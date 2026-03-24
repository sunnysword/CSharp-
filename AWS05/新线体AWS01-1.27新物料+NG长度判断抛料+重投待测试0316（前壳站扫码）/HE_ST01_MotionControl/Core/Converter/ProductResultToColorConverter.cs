using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Handler.Core.Converter
{
    public class ProductResultToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) return null;
            string result = (string)value;
            if (result == "1" || result.ToUpper() == "OK" || result.ToUpper() == "PASS")
            {
                return System.Windows.Media.Brushes.LightGreen;
            }
            else if (result == "0" || result.ToUpper() == "NG" || result.ToUpper() == "FAIL")
            {
                return System.Windows.Media.Brushes.Red;
            }
            {
                return System.Windows.Media.Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
