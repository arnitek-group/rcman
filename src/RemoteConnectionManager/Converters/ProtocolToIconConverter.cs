using MahApps.Metro.IconPacks;
using RemoteConnectionManager.Core;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RemoteConnectionManager.Converters
{
    public class ProtocolToIconConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var protocol = (Protocol) value;
            switch (protocol)
            {
                case Protocol.Rdp:
                    return PackIconMaterialKind.Monitor;
                case Protocol.Ssh:
                    return PackIconMaterialKind.Console;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
