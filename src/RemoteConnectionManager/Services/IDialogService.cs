
namespace RemoteConnectionManager.Services
{
    public interface IDialogService
    {
        void ShowInfoDialog(string text);
        void ShowWarningDialog(string text);
        bool ShowConfirmationDialog(string text);
    }
}
