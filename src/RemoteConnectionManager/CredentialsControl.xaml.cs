using RemoteConnectionManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RemoteConnectionManager
{
    public partial class CredentialsControl : UserControl
    {
        public CredentialsControl()
        {
            InitializeComponent();
        }

        private void PasswordBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            passwordBox.Password = ViewModelLocator.Locator.Main.SelectedCredentials.Password;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            ViewModelLocator.Locator.Main.SelectedCredentials.Password = passwordBox.Password;
        }
    }
}
