using System;
using System.Windows;
using System.Windows.Controls;

namespace RemoteConnectionManager.Core.Connections
{
    public interface IConnection
    {
        ConnectionSettings ConnectionSettings { get; }

        void Connect();
        void Disconnect();
        void Destroy();
        bool IsConnected { get; }

        FrameworkElement UI { get; }
        ContextMenu ContextMenu { get; }

        event EventHandler<DisconnectReason> Disconnected;
    }
}
