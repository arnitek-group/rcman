using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Core.Services;
using RemoteConnectionManager.Extensions;
using RemoteConnectionManager.ExternalProcess;
using RemoteConnectionManager.Properties;
using RemoteConnectionManager.Rdp;
using RemoteConnectionManager.Services;

namespace RemoteConnectionManager.ViewModels
{
    public class ImportViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ITelemetryService _telemetryService;

        public ImportViewModel(IDialogService dialogService, ITelemetryService telemetryService)
        {
            _dialogService = dialogService;
            _telemetryService = telemetryService;

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

                _telemetryService.TrackEvent("Import", new Dictionary<string, string>
                {
                    {"Protocol", items[0].ConnectionSettings.Protocol.ToString().ToUpper()},
                    {"Count", items.Length.ToString()}
                });
            }

            _dialogService.ShowInfoDialog(string.Format(Resources.InfoImport, items.Length));
        }
    }
}
