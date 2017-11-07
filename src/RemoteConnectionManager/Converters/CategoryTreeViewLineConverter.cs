using RemoteConnectionManager.Controls;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace RemoteConnectionManager.Converters
{
    public class CategoryTreeViewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (CategoryTreeViewItem)value;
            var ic = ItemsControl.ItemsControlFromItemContainer(item);
            return ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
