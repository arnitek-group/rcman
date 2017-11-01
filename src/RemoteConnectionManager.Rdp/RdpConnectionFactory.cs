using RemoteConnectionManager.Core;

namespace RemoteConnectionManager.Rdp
{
    public class RdpConnectionFactory : IConnectionFactory
    {
        public Protocol[] Protocols => new[] {Protocol.Rdp};

        public IConnection CreateConnection(ConnectionSettings connectionSettings)
        {
            return new RdpConnection(connectionSettings);
        }
    }
}
