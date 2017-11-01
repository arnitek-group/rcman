using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace RemoteConnectionManager.Services
{
    public class JsonSettingsService : ISettingsService
    {
        private const string SettingsFile = "connections.json";

        public Settings LoadSettings()
        {
            if (!File.Exists(SettingsFile))
            {
                File.CreateText(SettingsFile);
                return null;
            }
            return JsonConvert.DeserializeObject<Settings>(File.ReadAllText(SettingsFile));
        }

        public void SaveSettings(Settings settings)
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
            File.WriteAllText(SettingsFile, settingsText);
        }
    }
}
