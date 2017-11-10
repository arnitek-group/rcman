using RemoteConnectionManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RemoteConnectionManager
{
    public partial class PropertiesControl : UserControl
    {
        public PropertiesControl()
        {
            InitializeComponent();
        }

        private void PasswordBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            var credentialsViewModel = ViewModelLocator.Locator.Main.SelectedItem?.Credentials;
            if (credentialsViewModel != null)
            {
                passwordBox.Password = credentialsViewModel.Password;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            var credentialsViewModel = ViewModelLocator.Locator.Main.SelectedItem?.Credentials;
            if (credentialsViewModel != null)
            {
                credentialsViewModel.Password = passwordBox.Password;
            }
        }
    }
}
