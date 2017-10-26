using RemoteConnectionManager.Core;
using RemoteConnectionManager.Properties;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace RemoteConnectionManager.Converters
{
    public class PropertyGridCredentialsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new ComboBoxItem() { Content = Resources.Clear };
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Credentials)
            {
                return value;
            }

            return null;
        }
    }
}
