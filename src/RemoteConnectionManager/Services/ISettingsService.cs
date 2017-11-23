using RemoteConnectionManager.Models;

namespace RemoteConnectionManager.Services
{
    public interface ISettingsService
    {
        ApplicationSettings ApplicationSettings { get; }
        void SaveSettings();

        UserConnections LoadConnections();
        UserConnections LoadConnections(string filePath);
        void SaveConnections(UserConnections userConnections);
    }
}
