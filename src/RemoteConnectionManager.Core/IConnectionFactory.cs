
namespace RemoteConnectionManager.Core
{
    public interface IConnectionFactory
    {
        Protocol[] Protocols { get; }
        IConnection CreateConnection(ConnectionSettings connectionSettings);
    }
}
