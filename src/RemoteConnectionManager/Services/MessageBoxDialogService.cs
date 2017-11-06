using RemoteConnectionManager.Properties;
using System.Windows;

namespace RemoteConnectionManager.Core
{
    public class MessageBoxDialogService: IDialogService
    {
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
