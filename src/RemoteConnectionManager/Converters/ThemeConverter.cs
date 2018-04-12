using RemoteConnectionManager.Models;
using System;
using System.Globalization;
using System.Windows.Data;
using Themes = Xceed.Wpf.AvalonDock.Themes;

namespace RemoteConnectionManager.Converters
{
    public class ThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var theme = (Theme)value;
            switch(theme)
            {
                case Theme.Generic:
                    return new Themes.GenericTheme();
                case Theme.Aero:
                    return new Themes.AeroTheme();
                case Theme.Metro:
                    return new Themes.MetroTheme();
                case Theme.VS2010:
                    return new Themes.VS2010Theme();
                case Theme.VS2013Blue:
                    return new Themes.Vs2013BlueTheme();
                case Theme.VS2013Light:
                    return new Themes.Vs2013LightTheme();
                case Theme.VS2013Dark:
                    return new Themes.Vs2013DarkTheme();
            }

            return new Themes.AeroTheme();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
