using GalaSoft.MvvmLight.Ioc;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RemoteConnectionManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            Title = Properties.Resources.Application + " v" + fileVersionInfo.FileVersion;
        }

        public ViewModelLocator Locator => SimpleIoc.Default.GetInstance<ViewModelLocator>();

        private void ListBox_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var connectionSettings = (ConnectionSettings)((FrameworkElement)sender).DataContext;
            Locator.Main.ExecuteConnectCommand(connectionSettings);
        }

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle || e.MiddleButton != MouseButtonState.Pressed)
            {
                return;
            }

            var connection = (IConnection)((FrameworkElement)sender).DataContext;
            Locator.Main.ExecuteDisconnectCommand(connection);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!Locator.Main.OnClosing())
            {
                e.Cancel = true;
            }
        }

        private void PasswordBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = (PasswordBox) sender;
            passwordBox.Password = Locator.Main.SelectedCredentials.Password;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            Locator.Main.SelectedCredentials.Password = passwordBox.Password;
        }
    }
}
