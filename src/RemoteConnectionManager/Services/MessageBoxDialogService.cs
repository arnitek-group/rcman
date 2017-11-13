using System.Windows;
using RemoteConnectionManager.Properties;

namespace RemoteConnectionManager.Services
{
    public class MessageBoxDialogService: IDialogService
    {
        public void ShowInfoDialog(string text)
        {
            MessageBox.Show(
                text, Resources.Application,
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowWarningDialog(string text)
        {
            MessageBox.Show(
                text, Resources.Application,
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool ShowConfirmationDialog(string text)
        {
            var result = MessageBox.Show(
                text, Resources.Application,
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
