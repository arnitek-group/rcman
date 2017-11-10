using RemoteConnectionManager.Extensions;
using RemoteConnectionManager.Properties;
using RemoteConnectionManager.ViewModels;
using RemoteConnectionManager.ViewModels.Properties;
using System;
using System.Globalization;
using System.Linq;
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
                return new ComboBoxItem { Content = Resources.Clear };
            }

            return ViewModelLocator.Locator
                .Main.Items
                .GetFlatList(x => x.Items, x => x.Credentials != null)
                .Select(x => x.Credentials)
                .FirstOrDefault(x => x == value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CredentialsViewModel credentials)
            {
                return credentials;
            }

            return null;
        }
    }
}
