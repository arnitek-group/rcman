using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.ViewModels;

namespace RemoteConnectionManager
{
    public partial class ConnectionSettingsControl : UserControl
    {
        public ConnectionSettingsControl()
        {
            InitializeComponent();
        }

        private void ListBox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var connectionSettings = (ConnectionSettings)((FrameworkElement)sender).DataContext;
            ViewModelLocator.Locator.Main.ExecuteConnectCommand(connectionSettings);
        }
    }
}
