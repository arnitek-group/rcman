using RemoteConnectionManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RemoteConnectionManager.UserControls
{
    public partial class PropertyPassword : UserControl
    {
        public PropertyPassword()
        {
            InitializeComponent();
        }

        private void PasswordBox_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModelLocator.Locator.Main.SelectedItem?.Credentials != null)
            {
                PasswordBox_Password.Password = ViewModelLocator.Locator.Main.SelectedItem.Credentials.Password;
            }
            else if (ViewModelLocator.Locator.Main.SelectedItem?.ConnectionSettings != null)
            {
                PasswordBox_Password.Password = ViewModelLocator.Locator.Main.SelectedItem.ConnectionSettings.Password;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (ViewModelLocator.Locator.Main.SelectedItem?.Credentials != null)
            {
                ViewModelLocator.Locator.Main.SelectedItem.Credentials.Password = PasswordBox_Password.Password;
            }
            else if (ViewModelLocator.Locator.Main.SelectedItem?.ConnectionSettings != null)
            {
                ViewModelLocator.Locator.Main.SelectedItem.ConnectionSettings.Password = PasswordBox_Password.Password;
            }
        }
    }
}
