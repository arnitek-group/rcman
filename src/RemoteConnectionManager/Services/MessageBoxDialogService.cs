using RemoteConnectionManager.Properties;
using System.Windows;

namespace RemoteConnectionManager.Services
{
    public class MessageBoxDialogService: IDialogService
    {
        public bool ShowConfirmationDialog(string text)
        {
            var result = MessageBox.Show(
                text, Resources.Application,
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
