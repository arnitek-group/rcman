using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Models;
using System;
using System.IO;

namespace RemoteConnectionManager.Services
{
    public class JsonSettingsService : ISettingsService
    {

#if DEBUG
        private const string AppFolder = "RCManager-Debug";
#else
        private const string AppFolder = "RCManager";
#endif

        private readonly string _settingsFilePath;
        private readonly ApplicationSettings _applicationSettings;

        public JsonSettingsService()
        {
            _settingsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppFolder,
                "settings.json");

            _applicationSettings = LoadObjectFromFile<ApplicationSettings>(_settingsFilePath)
                ?? new ApplicationSettings
                {
                    Width = double.NaN,
                    Height = double.NaN,
                    WindowState = System.Windows.WindowState.Maximized,
                    Theme = Theme.Aero
                };

            // Upgrading from older version is updated.
            if (string.IsNullOrEmpty(_applicationSettings.LayoutFile))
            {
                _applicationSettings.LayoutFile = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        AppFolder, "layout.xml");
            }
            if (string.IsNullOrEmpty(_applicationSettings.ConnectionsFile))
            {
                _applicationSettings.ConnectionsFile = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        AppFolder, "connections.json");
            }
        }

        public ApplicationSettings ApplicationSettings => _applicationSettings;

        public void SaveSettings()
        {
            SaveObjectToFile(_applicationSettings, _settingsFilePath);
        }

        public UserConnections LoadConnections()
        {
            return LoadConnections(_applicationSettings.ConnectionsFile);
        }

        public UserConnections LoadConnections(string filePath)
        {
            return LoadObjectFromFile<UserConnections>(filePath)
                ?? new UserConnections() { Items = new CategoryItem[] { } };
        }

        public void SaveConnections(UserConnections userConnections)
        {
            SaveObjectToFile(userConnections, _applicationSettings.ConnectionsFile);
        }

        private T LoadObjectFromFile<T>(string file) where T : class
        {
            if (!File.Exists(file))
            {
                var directory = Path.GetDirectoryName(file);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.CreateText(file);
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
            }
            catch
            {
                return null;
            }
        }

        private void SaveObjectToFile(object @object, string file)
        {
            var settingsText = JsonConvert.SerializeObject(
                @object, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Converters = new[] { new StringEnumConverter() },
                    NullValueHandling = NullValueHandling.Ignore
                }
            );
            File.WriteAllText(file, settingsText);
        }
    }
}
