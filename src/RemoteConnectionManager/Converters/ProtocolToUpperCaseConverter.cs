using RemoteConnectionManager.Core;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RemoteConnectionManager.Converters
{
    public class ProtocolToUpperCaseConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return null;
            }
            
            return Enum.GetName(typeof(Protocol), value).ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
