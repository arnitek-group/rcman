using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Core.Services;
using RemoteConnectionManager.ExternalProcess;
using RemoteConnectionManager.Properties;
using RemoteConnectionManager.Rdp;
using RemoteConnectionManager.Services;
using System.Collections.Generic;

namespace RemoteConnectionManager.ViewModels
{
    public class ImportViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;
        private readonly ITelemetryService _telemetryService;

        public ImportViewModel(
            IDialogService dialogService,
            ISettingsService settingsService,
            ITelemetryService telemetryService)
        {
            _dialogService = dialogService;
            _settingsService = settingsService;
            _telemetryService = telemetryService;

            ImportRcManCommand = new RelayCommand(ExecuteImportRcManCommand);
            ImportRdpFileCommand = new RelayCommand(ExecuteImportRdpFileCommand);
            ImportPuttySessionsCommand = new RelayCommand(ExecuteImportPuttySessionsCommand);
        }

        public RelayCommand ImportRcManCommand { get; }
        private void ExecuteImportRcManCommand()
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "**.*|*.*";

            if (dialog.ShowDialog() == true)
            {
                ImportItems(_settingsService.LoadConnections(dialog.FileName)?.Items, "RCMan");
            }
        }

        public RelayCommand ImportRdpFileCommand { get; }
        private void ExecuteImportRdpFileCommand()
        {
            ImportItems(RdpImport.ImportFiles(), "RDP File");
        }

        public RelayCommand ImportPuttySessionsCommand { get; }
        private void ExecuteImportPuttySessionsCommand()
        {
            ImportItems(PuTTYImport.ImportSessions(), "PuTTY Sessions");
        }

        private void ImportItems(CategoryItem[] items, string importType)
        {
            if (items == null)
            {
                return;
            }

            if (items.Length > 0)
            {
                ViewModelLocator.Locator.Main.SuspendSave = true;
                ViewModelLocator.Locator.Main.LoadConnections(items);
                ViewModelLocator.Locator.Main.SuspendSave = false;
                ViewModelLocator.Locator.Main.SaveConnections();

                _telemetryService.TrackEvent("Import", new Dictionary<string, string>
                {
                    {"Type", importType}
                });
            }

            _dialogService.ShowInfoDialog(string.Format(Resources.InfoImport, items.Length));
        }
    }
}
