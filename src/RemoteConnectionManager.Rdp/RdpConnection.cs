using RemoteConnectionManager.Core;
using RemoteConnectionManager.Rdp.Properties;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace RemoteConnectionManager.Rdp
{
    public class RdpConnection : IConnection
    {
        public RdpConnection(ConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;
        }

        public bool IsConnected { get; private set; }
        public ConnectionSettings ConnectionSettings { get; }
        public FrameworkElement UI { get; private set; }

        public event EventHandler<DisconnectReason> Disconnected;

        private RdpHost _hostRdp;
        private Grid _hostGrid;

        public void Connect()
        {
            if (!IsConnected)
            {
                if (_hostRdp == null)
                {
                    _hostRdp = new RdpHost();
                    _hostRdp.AxMsRdpClient.Server = ConnectionSettings.Server;
                    _hostRdp.AxMsRdpClient.OnDisconnected += AxMsRdpClient_OnDisconnected;
                }

                var credentials = ConnectionSettings.Credentials;
                if (credentials != null)
                {
                    if (!string.IsNullOrEmpty(credentials.Domain))
                    {
                        _hostRdp.AxMsRdpClient.Domain = credentials.Domain;
                    }
                    if (!string.IsNullOrEmpty(credentials.Username) && !string.IsNullOrEmpty(credentials.Password))
                    {
                        _hostRdp.AxMsRdpClient.UserName = credentials.Username;
                        _hostRdp.AxMsRdpClient.AdvancedSettings2.ClearTextPassword = credentials.GetPassword();
                    }
                }

                _hostRdp.AxMsRdpClient.AdvancedSettings2.SmartSizing = true;
                // Keyboard redirection settings.
                // https://msdn.microsoft.com/en-us/library/aa381095(v=vs.85).aspx
                // https://msdn.microsoft.com/en-us/library/aa381299(v=vs.85).aspx
                _hostRdp.AxMsRdpClient.SecuredSettings2.KeyboardHookMode = 1;
                _hostRdp.AxMsRdpClient.AdvancedSettings2.EnableWindowsKey = 1;
                _hostRdp.AxMsRdpClient.AdvancedSettings7.EnableCredSspSupport = true;
                // Connection settings.
                _hostRdp.AxMsRdpClient.ConnectingText = Resources.Connecting + " " + ConnectionSettings.Server;
                _hostRdp.AxMsRdpClient.DisconnectedText = Resources.Disconnected + " " + ConnectionSettings.Server;
                
                if (_hostGrid == null)
                {
                    _hostGrid = new Grid();
                    _hostGrid.Children.Add(new WindowsFormsHost {Child = _hostRdp});
                    _hostGrid.SizeChanged += Host_SizeChanged_Initial;
                    _hostGrid.SizeChanged += Host_SizeChanged;
                }
                else
                {
                    PrepareSessionDisplaSettingsAndConnect();
                }

                UI = _hostGrid;

                IsConnected = true;
            }
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                if (_hostRdp.AxMsRdpClient.Connected == 1)
                {
                    _hostRdp.AxMsRdpClient.Disconnect();
                }
                IsConnected = false;
            }
        }

        public void Destroy()
        {
            if (_hostGrid != null)
            {
                _hostGrid.SizeChanged -= Host_SizeChanged_Initial;
                _hostGrid.SizeChanged -= Host_SizeChanged;
                _hostGrid.Dispatcher.Invoke(() => _hostGrid.Children.Clear());
                _hostGrid = null;
            }

            if (_hostRdp != null)
            {
                _hostRdp.AxMsRdpClient.OnDisconnected -= AxMsRdpClient_OnDisconnected;
                if (_hostRdp.AxMsRdpClient.Connected == 1)
                {
                    _hostRdp.AxMsRdpClient.Disconnect();
                }
                _hostRdp.Invoke((MethodInvoker)delegate { _hostRdp.Dispose(); });
                _hostRdp = null;
            }

            UI = null;
        }

        private void Host_SizeChanged_Initial(object sender, SizeChangedEventArgs e)
        {
            PrepareSessionDisplaSettingsAndConnect();
            _hostGrid.SizeChanged -= Host_SizeChanged_Initial;
        }

        private void Host_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSessionDisplaySettings();
        }

        private void AxMsRdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            var reason = DisconnectReason.ConnectionTerminated;
            // https://social.technet.microsoft.com/wiki/contents/articles/37870.rds-remote-desktop-client-disconnect-codes-and-reasons.aspx
            // https://msdn.microsoft.com/en-us/library/aa382170%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
            switch (e.discReason)
            {
                case Reason_ClientDisconnect:
                case Reason_ServerDisconnect:
                    reason = DisconnectReason.ConnectionEnded;
                    break;
                case Reason_DisconnectedByUser:
                    reason = DisconnectReason.KickedOut;
                    break;
                case Reason_ServerNotFound:
                    reason = DisconnectReason.ServerNotFound;
                    break;
                case Reason_TimedOut:
                    reason = DisconnectReason.ConnectionTimedOut;
                    break;
            }
            Disconnected?.Invoke(this, reason);
        }

        private void PrepareSessionDisplaSettingsAndConnect()
        {
            _hostRdp.AxMsRdpClient.DesktopWidth = _hostRdp.Width;
            _hostRdp.AxMsRdpClient.DesktopHeight = _hostRdp.Height;
            _hostRdp.AxMsRdpClient.Connect();
        }

        private void UpdateSessionDisplaySettings()
        {
            if (_hostRdp.AxMsRdpClient.Connected == 1)
            {
                _hostRdp.AxMsRdpClient.UpdateSessionDisplaySettings(
                    (uint)_hostRdp.Width, (uint)_hostRdp.Height,
                    (uint)_hostRdp.Width, (uint)_hostRdp.Height,
                    0, 1, 1);
            }
        }

        private const int Reason_ClientDisconnect = 0x1;
        private const int Reason_DisconnectedByUser = 0x2;
        private const int Reason_ServerDisconnect = 0x3;
        private const int Reason_ServerNotFound = 0x104;
        private const int Reason_TimedOut = 0x108;
    }
}
