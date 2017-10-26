using System;
using System.Windows;

namespace RemoteConnectionManager.Core
{
    public interface IConnection
    {
        ConnectionSettings ConnectionSettings { get; }
        void Connect();
        void Disconnect();
        bool IsConnected { get; }
        FrameworkElement UI { get; }

        event EventHandler Terminated;
    }
}
