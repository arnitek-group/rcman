
namespace RemoteConnectionManager.Services
{
    public interface ISettingsService
    {
        Settings LoadSettings();
        void SaveSettings(Settings settings);
    }
}
