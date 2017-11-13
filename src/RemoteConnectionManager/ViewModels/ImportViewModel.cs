using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Extensions;
using RemoteConnectionManager.ExternalProcess;
using RemoteConnectionManager.Properties;
using RemoteConnectionManager.Rdp;
using RemoteConnectionManager.Services;

namespace RemoteConnectionManager.ViewModels
{
    public class ImportViewModel: ViewModelBase
    {
        private readonly IDialogService _dialogService;
        public ImportViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            ImportRdpFileCommand = new RelayCommand(ExecuteImportRdpFileCommand);
            ImportPuttySessionsCommand = new RelayCommand(ExecuteImportPuttySessionsCommand);
        }

        public RelayCommand ImportRdpFileCommand { get; }
        private void ExecuteImportRdpFileCommand()
        {
            ImportItems(RdpImport.ImportFiles());
        }

        public RelayCommand ImportPuttySessionsCommand { get; }
        private void ExecuteImportPuttySessionsCommand()
        {
            ImportItems(PuTTYImport.ImportSessions());
        }

        private void ImportItems(CategoryItem[] items)
        {
            if (items == null)
            {
                return;
            }

            if (items.Length > 0)
            {
                ViewModelLocator.Locator.Main.SuspendSave = true;
                items.ForEach(x => ViewModelLocator.Locator.Main.Items.Add(new CategoryItemViewModel(x, null)));
                ViewModelLocator.Locator.Main.SuspendSave = false;
                ViewModelLocator.Locator.Main.SaveConnections();
            }

            _dialogService.ShowInfoDialog(string.Format(Resources.InfoImport, items.Length));
        }
    }
}
