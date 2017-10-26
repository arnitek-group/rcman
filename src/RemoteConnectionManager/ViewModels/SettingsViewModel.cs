using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RemoteConnectionManager.Core;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace RemoteConnectionManager.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private const string SettingsFile = "settings.json";
        private readonly MainViewModel _mainViewModel;

        public SettingsViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            if (!File.Exists(SettingsFile))
            {
                File.CreateText(SettingsFile);
            }
            else
            {
                var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFile));
                if (settings != null)
                {
                    foreach (var credentials in settings.Credentials)
                    {
                        credentials.PropertyChanged += Object_PropertyChanged;
                        _mainViewModel.Credentials.Add(credentials);
                    }
                    foreach (var connectionSettings in settings.ConnectionSettings)
                    {
                        connectionSettings.PropertyChanged += Object_PropertyChanged;
                        _mainViewModel.ConnectionSettings.Add(connectionSettings);
                    }
                }
            }

            _mainViewModel.Credentials.CollectionChanged += CollectionChanged;
            _mainViewModel.ConnectionSettings.CollectionChanged += CollectionChanged;
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
                foreach (var newItem in e.NewItems)
                {
                    ((INotifyPropertyChanged)newItem).PropertyChanged -= Object_PropertyChanged;
                }
            }
            SaveSettings();
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var settings = new Settings
            {
                Credentials = _mainViewModel.Credentials.ToArray(),
                ConnectionSettings = _mainViewModel.ConnectionSettings.ToArray()
            };
            var settingsText = JsonConvert.SerializeObject(
                settings, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Converters = new[] { new StringEnumConverter() }
                }
            );
            File.WriteAllText(SettingsFile, settingsText);
        }
    }

    public class Settings
    {
        public Credentials[] Credentials { get; set; }
        public ConnectionSettings[] ConnectionSettings { get; set; }
    }
}
