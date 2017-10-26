using GalaSoft.MvvmLight.Ioc;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.ViewModels;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace RemoteConnectionManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListBox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var connectionSettings = (ConnectionSettings)((FrameworkElement)sender).DataContext;
            SimpleIoc.Default.GetInstance<MainViewModel>().ExecuteConnectCommand(connectionSettings);
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle || e.MiddleButton != MouseButtonState.Pressed)
            {
                return;
            }

            var connection = (IConnection)((FrameworkElement)sender).DataContext;
            SimpleIoc.Default.GetInstance<MainViewModel>().ExecuteDisconnectCommand(connection);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!SimpleIoc.Default.GetInstance<MainViewModel>().OnClosing())
            {
                e.Cancel = true;
            }
        }
    }
}
