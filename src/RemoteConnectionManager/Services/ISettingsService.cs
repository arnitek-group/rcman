using RemoteConnectionManager.Models;

namespace RemoteConnectionManager.Services
{
    public interface ISettingsService
    {
        ApplicationSettings LoadSettings();
        void SaveSettings(ApplicationSettings applicationSettings);

        UserConnections LoadConnections();
        void SaveConnections(UserConnections userConnections);
    }
}
