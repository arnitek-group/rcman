using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Core.Services;
using RemoteConnectionManager.Extensions;
using RemoteConnectionManager.Models;
using RemoteConnectionManager.Properties;
using RemoteConnectionManager.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace RemoteConnectionManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ITelemetryService _telemetryService;
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialogService;

        public MainViewModel(
            ITelemetryService telemetryService,
            ISettingsService settingsService,
            IDialogService dialogService)
        {
            _telemetryService = telemetryService;
            _settingsService = settingsService;
            _dialogService = dialogService;
            
            Items = new ObservableCollection<CategoryItemViewModel>();
            Items.CollectionChanged += CollectionChanged;

            LoadConnections();

            CreateConnectionSettingsCommand = new RelayCommand(ExecuteCreateConnectionSettingsCommand);
            CreateCredentialsCommand = new RelayCommand(ExecuteCreateCredentialsCommand);
            CreateCategoryCommand = new RelayCommand(ExecuteCreateCategoryCommand);
            DeleteItemCommand = new RelayCommand(
                ExecuteDeleteItemCommand,
                CanExecuteDeleteItemCommand);
        }

        private void LoadConnections()
        {
            SuspendSave = true;
            LoadSettingsRecursive(_settingsService.LoadConnections().Items, null).ForEach(x => Items.Add(x));

            // Map credentials.
            var connectionSettingsList = Items.GetFlatList(x => x.Items, x => x.ConnectionSettings != null);
            var credentialsList = Items.GetFlatList(x => x.Items, x => x.Credentials != null);
            foreach (var connectionSettings in connectionSettingsList)
            {
                var credentials = credentialsList.FirstOrDefault(x =>
                    x.CategoryItem.Credentials == connectionSettings.CategoryItem.ConnectionSettings.Credentials);
                if (credentials != null)
                {
                    connectionSettings.ConnectionSettings.Credentials = credentials.Credentials;
                }
            }

            SuspendSave = false;
        }

        private CategoryItemViewModel[] LoadSettingsRecursive(IList<CategoryItem> items, CategoryItemViewModel parent)
        {
            if (items == null || items.Count == 0)
            {
                return new CategoryItemViewModel[] { };
            }

            var civms = items.Select(x => new CategoryItemViewModel(x, parent)).ToArray();
            foreach (var civm in civms)
            {
                civm.Items.CollectionChanged += CollectionChanged;
                var children = LoadSettingsRecursive(civm.CategoryItem.Items, civm);
                children.ForEach(x => civm.Items.Add(x));
            }

            return civms;
        }
        
        public ObservableCollection<CategoryItemViewModel> Items { get; }

        private CategoryItemViewModel _selectedItem;
        public CategoryItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    if (_selectedItem != null)
                    {
                        // Clear the selection from the old item.
                        _selectedItem.IsSelected = false;
                    }

                    _selectedItem = value;
                    RaisePropertyChanged();

                    if (_selectedItem != null)
                    {
                        // Select the new item.
                        _selectedItem.IsSelected = true;
                    }
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
            }, null);
            Items.Add(civm);

            SelectedItem = civm;

            _telemetryService.TrackEvent(
                "Create",
                new Dictionary<string, string>
                {
                    {"Type", "ConnectionSettings"}
                });
        }

        public RelayCommand CreateCredentialsCommand { get; }
        public void ExecuteCreateCredentialsCommand()
        {
            var civm = new CategoryItemViewModel(new CategoryItem
            {
                DisplayName = Resources.New + " " + Resources.Credentials,
                Credentials = new Credentials()
            }, null);
            Items.Add(civm);

            SelectedItem = civm;

            _telemetryService.TrackEvent(
                "Create",
                new Dictionary<string, string>
                {
                    {"Type", "Credentials"}
                });
        }

        public RelayCommand CreateCategoryCommand { get; }
        public void ExecuteCreateCategoryCommand()
        {
            var civm = new CategoryItemViewModel(new CategoryItem
            {
                DisplayName = Resources.New + " " + Resources.Category
            }, null);
            Items.Add(civm);

            SelectedItem = civm;

            _telemetryService.TrackEvent(
                "Create",
                new Dictionary<string, string>
                {
                    {"Type", "Category"}
                });
        }

        public RelayCommand DeleteItemCommand { get; }
        public bool CanExecuteDeleteItemCommand()
        {
            return SelectedItem != null;
        }
        public void ExecuteDeleteItemCommand()
        {
            var text = string.Format(Resources.ConfirmDelete, SelectedItem.DisplayName);
            if (!_dialogService.ShowConfirmationDialog(text))
            {
                return;
            }

            var deleteConnectionSettings = (Action<CategoryItemViewModel>)(civm =>
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
            });
            var deleteCredentials = (Action<CategoryItemViewModel>)(civm =>
            {
                var credentials = civm.CategoryItem.Credentials;
                Items
                    .GetFlatList(x => x.Items, x => x.CategoryItem.ConnectionSettings?.Credentials == credentials)
                    .ForEach(x => x.ConnectionSettings.Credentials = null);
            });

            SuspendSave = true;
            if (SelectedItem.ConnectionSettings != null)
            {
                deleteConnectionSettings(SelectedItem);
            }
            if (SelectedItem.Credentials != null)
            {
                deleteCredentials(SelectedItem);
            }
            if (SelectedItem.ConnectionSettings == null && SelectedItem.Credentials == null)
            {
                SelectedItem.Items
                    .GetFlatList(x => x.Items, x => x.ConnectionSettings != null)
                    .ForEach(x => deleteConnectionSettings(x));
                SelectedItem.Items
                    .GetFlatList(x => x.Items, x => x.Credentials != null)
                    .ForEach(x => deleteCredentials(x));
            }

            var oldSelectedItem = SelectedItem;
            SelectedItem = null;
            
            if (oldSelectedItem.Parent != null)
            {
                oldSelectedItem.Parent.CategoryItem.Items.Remove(oldSelectedItem.CategoryItem);
                oldSelectedItem.Parent.Items.Remove(oldSelectedItem);
            }
            else
            {
                Items.Remove(oldSelectedItem);
            }

            SuspendSave = false;
            SaveConnections();
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var newItem in e.NewItems)
                {
                    var civm = (CategoryItemViewModel)newItem;
                    civm.PropertyChanged += Object_PropertyChanged;
                    if (civm.Properties != null)
                    {
                        civm.Properties.PropertyChanged += Object_PropertyChanged;
                    }
                    civm.Items.CollectionChanged += CollectionChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var oldItem in e.OldItems)
                {
                    var civm = (CategoryItemViewModel)oldItem;
                    civm.PropertyChanged -= Object_PropertyChanged;
                    if (civm.Properties != null)
                    {
                        civm.Properties.PropertyChanged -= Object_PropertyChanged;
                    }
                    civm.Items.CollectionChanged -= CollectionChanged;
                }
            }
            SaveConnections();
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
                SaveConnections();
            }
        }

        public bool SuspendSave { get; set; }
        public void SaveConnections()
        {
            if (SuspendSave)
            {
                return;
            }

            _settingsService.SaveConnections(new UserConnections
            {
                Items = Items.Select(x => x.CategoryItem).ToArray()
            });
        }
    }
}
