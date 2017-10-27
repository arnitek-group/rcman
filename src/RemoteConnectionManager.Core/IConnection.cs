using System;
using System.Windows;

namespace RemoteConnectionManager.Core
{
    public interface IConnection
    {
        ConnectionSettings ConnectionSettings { get; }
        void Connect();
        void Disconnect();
        void Destroy();
        bool IsConnected { get; }
        FrameworkElement UI { get; }

        event EventHandler<DisconnectReason> Disconnected;
    }
}
