using RemoteConnectionManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace RemoteConnectionManager.Converters
{
    public class CredentialsFilterConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = (IEnumerable<CategoryItemViewModel>) value;
            return items
                .Where(x => x.Credentials != null)
                .Select(x => x.Credentials);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
