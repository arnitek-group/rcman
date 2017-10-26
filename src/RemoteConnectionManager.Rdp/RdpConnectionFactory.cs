using RemoteConnectionManager.Core;

namespace RemoteConnectionManager.Rdp
{
    public class RdpConnectionFactory : IConnectionFactory
    {
        public bool CanConnect(ConnectionSettings connectionSettings)
        {
            return connectionSettings.Protocol == Protocol.Rdp;
        }

        public IConnection CreateConnection(ConnectionSettings connectionSettings)
        {
            return new RdpConnection(connectionSettings);
        }
    }
}
