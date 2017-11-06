
namespace RemoteConnectionManager.Core
{
    public interface IDialogService
    {
        void ShowWarningDialog(string text);
        bool ShowConfirmationDialog(string text);
    }
}
