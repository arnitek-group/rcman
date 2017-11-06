using RemoteConnectionManager.Core;
using RemoteConnectionManager.Core.Connections;

namespace RemoteConnectionManager.ExternalProcess
{
    public class PuTTYConnectionFactory : IConnectionFactory
    {
        public Protocol[] Protocols => new[] { Protocol.Ssh };

        public IConnection CreateConnection(ConnectionSettings connectionSettings)
        {
            return new PuTTYConnection(connectionSettings);
        }
    }
}
