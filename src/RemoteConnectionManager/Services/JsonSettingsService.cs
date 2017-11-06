using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;

namespace RemoteConnectionManager.Core
{
    public class JsonSettingsService : ISettingsService
    {

#if DEBUG
        private const string AppFolder = "RCManager-Debug";
#else
        private const string AppFolder = "RCManager";
#endif
        private const string ConnectionsFileName = "connections.json";

        private readonly string _connectionsFilePath;

        public JsonSettingsService()
        {
            _connectionsFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppFolder,
                ConnectionsFileName);
        }

        public Settings LoadConnections()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_connectionsFilePath));
            if (!File.Exists(_connectionsFilePath))
            {
                File.CreateText(_connectionsFilePath);
                return null;
            }
            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_connectionsFilePath));
        }

        public void SaveConnections(Settings settings)
        {
            var settingsText = JsonConvert.SerializeObject(
                settings, Formatting.Indented,
                new JsonSerializerSettings
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    Converters = new[] { new StringEnumConverter() },
                    NullValueHandling = NullValueHandling.Ignore
                }
            );
            File.WriteAllText(_connectionsFilePath, settingsText);
        }
    }
}
