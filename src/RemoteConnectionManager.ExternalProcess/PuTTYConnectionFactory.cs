using RemoteConnectionManager.Core.Connections;
using System;

namespace RemoteConnectionManager.ExternalProcess
{
    public class PuTTYConnectionFactory : IConnectionFactory
    {
        public Protocol[] Protocols => new[] { Protocol.Ssh };

        public IConnection CreateConnection(ConnectionSettings connectionSettings, IntPtr topWindowHandle)
        {
            return new PuTTYConnection(connectionSettings, topWindowHandle);
        }
    }
}
