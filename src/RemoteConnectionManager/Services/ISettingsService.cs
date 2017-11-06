
namespace RemoteConnectionManager.Core
{
    public interface ISettingsService
    {
        Settings LoadConnections();
        void SaveConnections(Settings settings);
    }
}
