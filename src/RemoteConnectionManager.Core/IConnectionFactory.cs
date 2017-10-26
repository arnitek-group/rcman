
namespace RemoteConnectionManager.Core
{
    public interface IConnectionFactory
    {
        bool CanConnect(ConnectionSettings connectionSettings);
        IConnection CreateConnection(ConnectionSettings connectionSettings);
    }
}
