using RemoteConnectionManager.Models;

namespace RemoteConnectionManager.Services
{
    public interface ISettingsService
    {
        ApplicationSettings ApplicationSettings { get; }
        void SaveSettings();

        UserConnections LoadConnections();
        void SaveConnections(UserConnections userConnections);

    }
}
