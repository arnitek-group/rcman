using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.Properties;
using RemoteConnectionManager.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace RemoteConnectionManager.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        public SettingsViewModel(
            ISettingsService settingsService,
            IDialogService dialogService)
        {
            _settingsService = settingsService;
            _dialogService = dialogService;

            Credentials = new ObservableCollection<CredentialsViewModel>();
            ConnectionSettings = new ObservableCollection<ConnectionSettingsViewModel>();

            Credentials.CollectionChanged += CollectionChanged;
            ConnectionSettings.CollectionChanged += CollectionChanged;

            var settings = _settingsService.LoadSettings();
            if (settings != null)
            {
                foreach (var credentials in settings.Credentials)
                {
                    Credentials.Add(new CredentialsViewModel(credentials));
                }
                foreach (var connectionSettings in settings.ConnectionSettings)
                {
                    var csvm = new ConnectionSettingsViewModel(connectionSettings);
                    csvm.Credentials = Credentials.FirstOrDefault(x => x.Credentials == connectionSettings.Credentials);
                    ConnectionSettings.Add(csvm);
                }
            }

            CreateConnectionSettingsCommand = new RelayCommand(ExecuteCreateConnectionSettingsCommand);
            DeleteConnectionSettingsCommand = new RelayCommand(
                ExecuteDeleteConnectionSettingsCommand,
                CanExecuteDeleteConnectionSettingsCommand);

            CreateCredentialsCommand = new RelayCommand(ExecuteCreateCredentialsCommand);
            DeleteCredentialsCommand = new RelayCommand(
                ExecuteDeleteCredentialsCommand,
                CanExecuteDeleteCredentialsCommand);
        }

        public ObservableCollection<CredentialsViewModel> Credentials { get; }
        public ObservableCollection<ConnectionSettingsViewModel> ConnectionSettings { get; }

        public RelayCommand CreateConnectionSettingsCommand { get; }
        public void ExecuteCreateConnectionSettingsCommand()
        {
            var csvm = new ConnectionSettingsViewModel(new ConnectionSettings
            {
                DisplayName = Resources.New
            });
            ConnectionSettings.Add(csvm);
            ViewModelLocator.Locator.Connections.SelectedConnectionSettings = csvm;
        }

        public RelayCommand DeleteConnectionSettingsCommand { get; }
        public bool CanExecuteDeleteConnectionSettingsCommand()
        {
            return ViewModelLocator.Locator.Connections.SelectedConnectionSettings != null;
        }
        public void ExecuteDeleteConnectionSettingsCommand()
        {
            var connectionSettings = ViewModelLocator.Locator.Connections.SelectedConnectionSettings;

            var text = string.Format(Resources.ConfirmDelete, connectionSettings.DisplayName);
            if (!_dialogService.ShowConfirmationDialog(text))
            {
                return;
            }

            _isSaveSuspended = true;
            var connection = ViewModelLocator.Locator.Connections.Connections
                .FirstOrDefault(x => x.ConnectionSettings == connectionSettings.ConnectionSettings);
            if (connection != null)
            {
                connection.Disconnect();
                connection.Destroy();
                ViewModelLocator.Locator.Connections.Connections.Remove(connection);
            }
            ViewModelLocator.Locator.Connections.SelectedConnectionSettings = null;
            _isSaveSuspended = false;
            ConnectionSettings.Remove(connectionSettings);
        }

        public RelayCommand CreateCredentialsCommand { get; }
        public void ExecuteCreateCredentialsCommand()
        {
            var cvm = new CredentialsViewModel(new Credentials
            {
                DisplayName = Resources.New
            });
            Credentials.Add(cvm);
            ViewModelLocator.Locator.Connections.SelectedCredentials = cvm;
        }

        public RelayCommand DeleteCredentialsCommand { get; }
        public bool CanExecuteDeleteCredentialsCommand()
        {
            return ViewModelLocator.Locator.Connections.SelectedCredentials != null;
        }
        public void ExecuteDeleteCredentialsCommand()
        {
            var credentials = ViewModelLocator.Locator.Connections.SelectedCredentials;

            var text = string.Format(Resources.ConfirmDelete, credentials.DisplayName);
            if (!_dialogService.ShowConfirmationDialog(text))
            {
                return;
            }

            _isSaveSuspended = true;
            var connectionSettings = ConnectionSettings.Where(x => x.Credentials == credentials);
            foreach (var csvm in connectionSettings)
            {
                csvm.Credentials = null;
            }
            ViewModelLocator.Locator.Connections.SelectedCredentials = null;
            _isSaveSuspended = false;
            Credentials.Remove(credentials);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    ((INotifyPropertyChanged)newItem).PropertyChanged += Object_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    ((INotifyPropertyChanged)oldItem).PropertyChanged -= Object_PropertyChanged;
                }
            }
            SaveSettings();
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveSettings();
        }

        private bool _isSaveSuspended;

        private void SaveSettings()
        {
            if (_isSaveSuspended)
            {
                return;
            }

            _settingsService.SaveSettings(new Services.Settings
            {
                Credentials = Credentials.Select(x => x.Credentials).ToArray(),
                ConnectionSettings = ConnectionSettings.Select(x => x.ConnectionSettings).ToArray()
            });
        }
    }
}
