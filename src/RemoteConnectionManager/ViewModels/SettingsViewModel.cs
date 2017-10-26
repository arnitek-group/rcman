using System.Collections.ObjectModel;
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

        public SettingsViewModel()
        {
            Credentials = new ObservableCollection<Credentials>();
            ConnectionSettings = new ObservableCollection<ConnectionSettings>();

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
                        Credentials.Add(credentials);
                    }
                    foreach (var connectionSettings in settings.ConnectionSettings)
                    {
                        connectionSettings.PropertyChanged += Object_PropertyChanged;
                        ConnectionSettings.Add(connectionSettings);
                    }
                }
            }

            Credentials.CollectionChanged += CollectionChanged;
            ConnectionSettings.CollectionChanged += CollectionChanged;
        }

        public ObservableCollection<Credentials> Credentials { get; }
        public ObservableCollection<ConnectionSettings> ConnectionSettings { get; }

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
                Credentials = Credentials.ToArray(),
                ConnectionSettings = ConnectionSettings.ToArray()
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
