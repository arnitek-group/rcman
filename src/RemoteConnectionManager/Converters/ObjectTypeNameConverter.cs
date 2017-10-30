using RemoteConnectionManager.Properties;
using RemoteConnectionManager.ViewModels;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RemoteConnectionManager.Converters
{
    class ObjectTypeNameConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ConnectionSettingsViewModel)
            {
                return Resources.ConnectionSettings;
            }
            if (value is CredentialsViewModel)
            {
                return Resources.Credentials;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
