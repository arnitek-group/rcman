using RemoteConnectionManager.Core.Connections;
using System;

namespace RemoteConnectionManager.ExternalProcess
{
    public class PowerShellConnectionFactory : IConnectionFactory
    {
        public Protocol[] Protocols => new[] { Protocol.PowerShell };

        public IConnection CreateConnection(ConnectionSettings connectionSettings, IntPtr topWindowHandle)
        {
            return new PowerShellConnection(connectionSettings, topWindowHandle);
        }
    }
}
