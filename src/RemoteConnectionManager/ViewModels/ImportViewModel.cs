using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Extensions;
using RemoteConnectionManager.ExternalProcess;
using RemoteConnectionManager.Properties;
using RemoteConnectionManager.Services;

namespace RemoteConnectionManager.ViewModels
{
    public class ImportViewModel: ViewModelBase
    {
        private readonly IDialogService _dialogService;
        public ImportViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            ImportPuttySessionsCommand = new RelayCommand(ExecuteImportPuttySessionsCommand);
        }

        public RelayCommand ImportPuttySessionsCommand { get; }
        private void ExecuteImportPuttySessionsCommand()
        {
            var importedSessions = PuTTYImport.ImportSessions();
            if (importedSessions.Length > 0)
            {
                ViewModelLocator.Locator.Main.SuspendSave = true;
                importedSessions.ForEach(x => ViewModelLocator.Locator.Main.Items.Add(new CategoryItemViewModel(x, null)));                
                ViewModelLocator.Locator.Main.SuspendSave = false;
                ViewModelLocator.Locator.Main.SaveConnections();
            }

            _dialogService.ShowInfoDialog(string.Format(Resources.InfoImport, importedSessions.Length));
        }
    }
}
