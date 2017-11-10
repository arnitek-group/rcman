
using RemoteConnectionManager.Core;

namespace RemoteConnectionManager.Services
{
    public interface ISettingsService
    {
        Settings LoadConnections();
        void SaveConnections(Settings settings);
    }
}
