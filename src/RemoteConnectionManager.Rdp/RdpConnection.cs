using MSTSCLib;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Core.Interop;
using RemoteConnectionManager.Core.Services;
using RemoteConnectionManager.Rdp.Properties;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RemoteConnectionManager.Rdp
{
    public class RdpConnection : IConnection
    {
        private readonly ITelemetryService _telemetryService;
        private readonly IntPtr _topWindowHandle;

        public RdpConnection(ITelemetryService telemetryService, ConnectionSettings connectionSettings, IntPtr topWindowHandle)
        {
            _telemetryService = telemetryService;
            _topWindowHandle = topWindowHandle;

            ConnectionSettings = connectionSettings;
            ContextMenu = new System.Windows.Controls.ContextMenu();

            var menuFullscreen = new System.Windows.Controls.MenuItem();
            menuFullscreen.Header = Resources.Fullscreen;
            menuFullscreen.Icon = new Image { Source = new BitmapImage(new Uri("/RemoteConnectionManager.Rdp;component/Resources/VSO_FullScreen_16x.png", UriKind.Relative)) };
            menuFullscreen.Click += (sender, e) => ToggleFullscreen();

            var menuCtrlAltDel = new System.Windows.Controls.MenuItem();
            menuCtrlAltDel.Header = Resources.CtrlAltDel;
            menuCtrlAltDel.Click += (sender, e) => SendCtrlAltDel();

            _menuReconnect = new System.Windows.Controls.MenuItem();
            _menuReconnect.Header = Resources.Reconnect;
            _menuReconnect.Icon = new Image { Source = new BitmapImage(new Uri("/RemoteConnectionManager.Rdp;component/Resources/Refresh_grey_16x.png", UriKind.Relative)) };
            _menuReconnect.Click += (sender, e) => Reconnect();

            ContextMenu.Items.Add(menuFullscreen);
            ContextMenu.Items.Add(menuCtrlAltDel);
            ContextMenu.Items.Add(_menuReconnect);

            IsConnected = false;
        }

        public ConnectionSettings ConnectionSettings { get; }

        public FrameworkElement UI { get; private set; }
        public System.Windows.Controls.ContextMenu ContextMenu { get; }

        public event EventHandler<DisconnectReason> Disconnected;

        private readonly System.Windows.Controls.MenuItem _menuReconnect;
        private RdpHost _hostRdp;
        private Grid _hostGrid;

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            private set
            {
                _isConnected = value;
                _menuReconnect.IsEnabled = !_isConnected;
            }
        }

        public void Connect()
        {
            if (!IsConnected)
            {
                if (_hostRdp == null)
                {
                    _hostRdp = new RdpHost();
                    _hostRdp.AxMsRdpClient.Server = ConnectionSettings.Server;

                    if (int.TryParse(ConnectionSettings.Port, out var port))
                    {
                        _hostRdp.AxMsRdpClient.AdvancedSettings2.RDPPort = port;
                    }

                    _hostRdp.AxMsRdpClient.OnDisconnected += AxMsRdpClient_OnDisconnected;
                }

                var credentials = ConnectionSettings.GetCredentials();
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
                //_hostRdp.AxMsRdpClient.AdvancedSettings4.ConnectionBarShowMinimizeButton = false;
                // Keyboard redirection settings.
                // https://msdn.microsoft.com/en-us/library/aa381095(v=vs.85).aspx
                // https://msdn.microsoft.com/en-us/library/aa381299(v=vs.85).aspx
                _hostRdp.AxMsRdpClient.SecuredSettings2.KeyboardHookMode = 1;
                _hostRdp.AxMsRdpClient.AdvancedSettings.allowBackgroundInput = 1;
                _hostRdp.AxMsRdpClient.AdvancedSettings2.EnableWindowsKey = 1;
                CastAndExecute<IMsRdpClient7>(x => x.AdvancedSettings7.EnableCredSspSupport = true);
                // Connection settings.
                _hostRdp.AxMsRdpClient.ConnectingText = Resources.Connecting + " " + ConnectionSettings.Server;
                _hostRdp.AxMsRdpClient.DisconnectedText = Resources.Disconnected + " " + ConnectionSettings.Server;

                if (_hostGrid == null)
                {
                    _hostGrid = new Grid();
                    _hostGrid.Children.Add(new WindowsFormsHost { Child = _hostRdp });
                    _hostGrid.SizeChanged += Host_SizeChanged_Initial;

                    Observable
                        .FromEventPattern<SizeChangedEventArgs>(_hostGrid, "SizeChanged")
                        .Throttle(TimeSpan.FromSeconds(1))
                        .Subscribe(x => _hostGrid?.Dispatcher.Invoke(UpdateSessionDisplaySettings));
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
                _hostGrid.Dispatcher.Invoke(() => _hostGrid.Children.Clear());
                _hostGrid = null;
            }

            if (_hostRdp != null)
            {
                _hostRdp.AxMsRdpClient.OnDisconnected -= AxMsRdpClient_OnDisconnected;
                if (_hostRdp.AxMsRdpClient.Connected == 1 && IsConnected)
                {
                    _hostRdp.AxMsRdpClient.Disconnect();
                }
                _hostRdp.Invoke((MethodInvoker)delegate { _hostRdp.Dispose(); });
                _hostRdp = null;
            }

            UI = null;
        }

        #region RDP

        private void CastAndExecute<T>(Action<T> action) where T : IMsRdpClient
        {
            if (_hostRdp.AxMsRdpClient.GetOcx() is T ocx)
            {
                action(ocx);
            }
        }

        private void Host_SizeChanged_Initial(object sender, SizeChangedEventArgs e)
        {
            PrepareSessionDisplaSettingsAndConnect();
            _hostGrid.SizeChanged -= Host_SizeChanged_Initial;
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
            IsConnected = false;
            Disconnected?.Invoke(this, reason);
        }

        private void PrepareSessionDisplaSettingsAndConnect()
        {
            var size = GetClientSize();
            _hostRdp.AxMsRdpClient.DesktopWidth = (int)size.Width;
            _hostRdp.AxMsRdpClient.DesktopHeight = (int)size.Height;
            try
            {
                _hostRdp.AxMsRdpClient.Connect();
            }
            catch
            {
                IsConnected = false;
            }
            WindowsInterop.SetWindowPos(
                _topWindowHandle, IntPtr.Zero,
                0, 0, 0, 0, WindowsInterop.SWP_NOSIZE);
        }

        private void UpdateSessionDisplaySettings()
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                if (_hostRdp.AxMsRdpClient.Connected == 1)
                {
                    try
                    {
                        var size = GetClientSize();
                        CastAndExecute<IMsRdpClient9>(x => x.UpdateSessionDisplaySettings(
                            (uint)size.Width, (uint)size.Height,
                            (uint)size.Width, (uint)size.Height,
                            0, 1, 1));
                    }
                    catch
                    { }
                }
            }
            WindowsInterop.SetWindowPos(
                _topWindowHandle, IntPtr.Zero,
                0, 0, 0, 0, WindowsInterop.SWP_NOSIZE);
        }

        private const int Reason_ClientDisconnect = 0x1;
        private const int Reason_DisconnectedByUser = 0x2;
        private const int Reason_ServerDisconnect = 0x3;
        private const int Reason_ServerNotFound = 0x104;
        private const int Reason_TimedOut = 0x108;

        #endregion RDP

        #region Commands

        private void Reconnect()
        {
            Disconnect();
            Connect();
        }

        private void ToggleFullscreen()
        {
            _hostRdp.AxMsRdpClient.FullScreenTitle = ConnectionSettings.Server;
            _hostRdp.AxMsRdpClient.FullScreen = !_hostRdp.AxMsRdpClient.FullScreen;
            if (_hostRdp.AxMsRdpClient.Connected == 1)
            {
                try
                {
                    var size = GetClientSize();
                    CastAndExecute<IMsRdpClient9>(x => x.UpdateSessionDisplaySettings(
                        (uint)size.Width, (uint)size.Height,
                        (uint)size.Width, (uint)size.Height,
                        0, 1, 1));
                }
                catch
                { }
            }

            _telemetryService.TrackEvent("Command", new Dictionary<string, string>
            {
                { "Protocol", "RPD" },
                {"Type", "Fullscreen" }
            });
        }

        private Size GetClientSize()
        {
            if (_hostRdp.AxMsRdpClient.FullScreen)
            {
                var gridLocation = _hostGrid.PointToScreen(new Point(0, 0));
                var screen = Screen.FromPoint(new System.Drawing.Point((int)gridLocation.X, (int)gridLocation.Y));

                return new Size(screen.Bounds.Width, screen.Bounds.Height);
            }

            return new Size(_hostRdp.Width, _hostRdp.Height);
        }

        private void SendCtrlAltDel()
        {
            if (_hostRdp.AxMsRdpClient.Connected == 1)
            {
                // Source: https://github.com/bosima/RDManager/blob/master/RDManager/MainForm.cs
                _hostRdp.AxMsRdpClient.Focus();
                new MsRdpClientNonScriptableWrapper(_hostRdp.AxMsRdpClient.GetOcx()).SendKeys(
                    new int[] { 0x1d, 0x38, 0x53, 0x53, 0x38, 0x1d },
                    new bool[] { false, false, false, true, true, true, }
                );
            }

            _telemetryService.TrackEvent("Command", new Dictionary<string, string>
            {
                { "Protocol", "RPD" },
                {"Type", "CtrlAltDel" }
            });
        }

        #endregion
    }
}
