using RemoteConnectionManager.Properties;
using RemoteConnectionManager.ViewModels;
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
                .Settings.Credentials.Single(x => x == value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CredentialsViewModel model)
            {
                return model;
            }

            return null;
        }
    }
}
