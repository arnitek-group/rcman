using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.Extensions;
using RemoteConnectionManager.Models;
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

            Items = new ObservableCollection<CategoryItemViewModel>();
            Items.CollectionChanged += CollectionChanged;

            var settings = _settingsService.LoadSettings();
            if (settings != null)
            {
                var items = settings.Items
                    .Select(x => new CategoryItemViewModel(x))
                    .ToArray();
                var connectionSettings = items.Where(x => x.ConnectionSettings != null);
                foreach (var item in connectionSettings)
                {
                    var credentials = items.FirstOrDefault(x =>
                        x != item &&
                        x.CategoryItem.Credentials == item.CategoryItem.ConnectionSettings.Credentials);
                    if (credentials != null)
                    {
                        item.ConnectionSettings.Credentials = credentials.Credentials;
                    }
                }
                items.ForEach(x => Items.Add(x));
            }

            CreateConnectionSettingsCommand = new RelayCommand(ExecuteCreateConnectionSettingsCommand);
            CreateCredentialsCommand = new RelayCommand(ExecuteCreateCredentialsCommand);
            DeleteItemCommand = new RelayCommand(
                ExecuteDeleteItemCommand,
                CanExecuteDeleteItemCommand);
        }

        public ObservableCollection<CategoryItemViewModel> Items { get; }

        public CategoryItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged();
                }
                DeleteItemCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand CreateConnectionSettingsCommand { get; }
        public void ExecuteCreateConnectionSettingsCommand()
        {
            var civm = new CategoryItemViewModel(new CategoryItem
            {
                DisplayName = Resources.New + " " + Resources.ConnectionSettings,
                ConnectionSettings = new ConnectionSettings()
            });
            Items.Add(civm);

            SelectedItem = civm;
        }

        public RelayCommand CreateCredentialsCommand { get; }
        public void ExecuteCreateCredentialsCommand()
        {
            var civm = new CategoryItemViewModel(new CategoryItem
            {
                DisplayName = Resources.New + " " + Resources.Credentials,
                Credentials = new Credentials()
            });
            Items.Add(civm);

            SelectedItem = civm;
        }

        public RelayCommand DeleteItemCommand { get; }
        public bool CanExecuteDeleteItemCommand()
        {
            return SelectedItem != null;
        }
        public void ExecuteDeleteItemCommand()
        {
            var civm = SelectedItem;
            var text = string.Format(Resources.ConfirmDelete, civm.DisplayName);
            if (!_dialogService.ShowConfirmationDialog(text))
            {
                return;
            }

            _isSaveSuspended = true;
            if (civm.CategoryItem.ConnectionSettings != null)
            {
                var connectionSettings = civm.CategoryItem.ConnectionSettings;
                var connection = ViewModelLocator.Locator
                    .Connections.Connections
                    .FirstOrDefault(x => x.ConnectionSettings == connectionSettings);
                if (connection != null)
                {
                    connection.Disconnect();
                    connection.Destroy();
                    ViewModelLocator.Locator.Connections.Connections.Remove(connection);
                }
            }
            if (civm.CategoryItem.Credentials != null)
            {
                var credentials = civm.CategoryItem.Credentials;
                Items
                    .Where(x => x.CategoryItem.ConnectionSettings?.Credentials == credentials)
                    .ForEach(x => x.ConnectionSettings.Credentials = null);
            }
            _isSaveSuspended = false;

            SelectedItem = null;
            Items.Remove(civm);
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    var civm = (CategoryItemViewModel) newItem;
                    civm.PropertyChanged += Object_PropertyChanged;
                    civm.Properties.PropertyChanged += Object_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var civm = (CategoryItemViewModel)oldItem;
                    civm.PropertyChanged -= Object_PropertyChanged;
                    civm.Properties.PropertyChanged -= Object_PropertyChanged;
                }
            }
            SaveSettings();
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                if (sender is CategoryItemViewModel civm && civm.IsSelected)
                {
                    SelectedItem = civm;
                }
            }
            else
            {
                SaveSettings();
            }
        }

        private bool _isSaveSuspended;
        private CategoryItemViewModel _selectedItem;

        private void SaveSettings()
        {
            if (_isSaveSuspended)
            {
                return;
            }

            _settingsService.SaveSettings(new Services.Settings
            {
                Items = Items.Select(x => x.CategoryItem).ToArray()
            });
        }
    }
}
