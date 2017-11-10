using System;

namespace RemoteConnectionManager.Core.Connections
{
    public interface IConnectionFactory
    {
        Protocol[] Protocols { get; }
        IConnection CreateConnection(ConnectionSettings connectionSettings, IntPtr topWindowHandle);
    }
}
