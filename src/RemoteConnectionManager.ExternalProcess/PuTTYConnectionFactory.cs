using RemoteConnectionManager.Core;
using System;

namespace RemoteConnectionManager.ExternalProcess
{
    public class PuTTYConnectionFactory : IConnectionFactory
    {
        public bool CanConnect(ConnectionSettings connectionSettings)
        {
            return connectionSettings.Protocol == Protocol.Ssh;
        }

        public IConnection CreateConnection(ConnectionSettings connectionSettings)
        {
            return new PuTTYConnection(connectionSettings);
        }
    }
}
