using RemoteConnectionManager.Core.Connections;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RemoteConnectionManager.Converters
{
    public class CategoryItemToImageConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var categoryItem = (CategoryItem) value;
            if (categoryItem.ConnectionSettings != null)
            {
                return "Resources/RemoteDesktop_16x.png";
            }
            if (categoryItem.Credentials != null)
            {
                return "Resources/User_16x.png";
            }
            return "Resources/Tag_16x.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
