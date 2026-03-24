using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Handler.Core.Converter
{
    /// <summary>
    /// 产品状态信息 锁附显示
    /// </summary>
    public class ProductResultToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string result = (string)value;
            if (result == "1" || result.ToUpper() == "OK" || result.ToUpper() == "PASS")
            {
                return "OK";
            }
            else if (result == "0" || result.ToUpper() == "NG" || result.ToUpper() == "FAIL")
            {
                return "NG";
            }
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
