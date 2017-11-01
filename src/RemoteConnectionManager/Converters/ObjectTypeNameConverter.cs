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
            var civm = value as CategoryItemViewModel;
            if (civm == null)
            {
                return null;
            }
            if (civm.CategoryItem.ConnectionSettings != null)
            {
                return Resources.ConnectionSettings;
            }
            if (civm.CategoryItem.Credentials != null)
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
