using RemoteConnectionManager.Core;
using RemoteConnectionManager.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
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

        private void TabItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Middle || e.MiddleButton != MouseButtonState.Pressed)
            {
                return;
            }

            var connection = (IConnection)((FrameworkElement)sender).DataContext;
            ViewModelLocator.Locator.Main.ExecuteDisconnectCommand(connection);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!ViewModelLocator.Locator.Main.OnClosing())
            {
                e.Cancel = true;
            }
        }
    }
}
