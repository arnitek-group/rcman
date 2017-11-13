using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        private const string SettingsFileName = "settings.json";
        private const string ConnectionsFileName = "connections.json";

        private readonly string _settingsFilePath;
        private readonly string _connectionsFilePath;

        public JsonSettingsService()
        {
            _settingsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppFolder,
                SettingsFileName);
            _connectionsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppFolder,
                ConnectionsFileName);
        }

        public ApplicationSettings LoadSettings()
        {
            return LoadObjectFromFile<ApplicationSettings>(_settingsFilePath);
        }

        public void SaveSettings(ApplicationSettings applicationSettings)
        {
            SaveObjectToFile(applicationSettings, _settingsFilePath);
        }

        public UserConnections LoadConnections()
        {
            return LoadObjectFromFile<UserConnections>(_connectionsFilePath);
        }

        public void SaveConnections(UserConnections userConnections)
        {
            SaveObjectToFile(userConnections, _connectionsFilePath);
        }

        private T LoadObjectFromFile<T>(string file) where T : class
        {
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            if (!File.Exists(file))
            {
                File.CreateText(file);
                return null;
            }
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
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
