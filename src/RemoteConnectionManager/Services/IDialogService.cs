
namespace RemoteConnectionManager.Services
{
    public interface IDialogService
    {
        void ShowWarningDialog(string text);
        bool ShowConfirmationDialog(string text);
    }
}
