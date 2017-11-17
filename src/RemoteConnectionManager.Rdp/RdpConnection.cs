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
                    _hostRdp.Rdp.Server = ConnectionSettings.Server;

                    if (int.TryParse(ConnectionSettings.Port, out var port))
                    {
                        _hostRdp.Rdp.Port = port;
                    }

                    _hostRdp.Rdp.OnDisconnected += Rdp_OnDisconnected;
                }

                var credentials = ConnectionSettings.GetCredentials();
                if (credentials != null)
                {
                    if (!string.IsNullOrEmpty(credentials.Domain))
                    {
                        _hostRdp.Rdp.Domain = credentials.Domain;
                    }
                    if (!string.IsNullOrEmpty(credentials.Username) && !string.IsNullOrEmpty(credentials.Password))
                    {
                        _hostRdp.Rdp.UserName = credentials.Username;
                        _hostRdp.Rdp.Password = credentials.GetPassword();
                    }
                }
                
                // Connection settings.
                _hostRdp.Rdp.ConnectingText = Resources.Connecting + " " + ConnectionSettings.Server;
                _hostRdp.Rdp.DisconnectedText = Resources.Disconnected + " " + ConnectionSettings.Server;

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
                if (_hostRdp.Rdp.Connected)
                {
                    _hostRdp.Rdp.Disconnect();
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
                _hostRdp.Rdp.OnDisconnected -= Rdp_OnDisconnected;
                if (_hostRdp.Rdp.Connected && IsConnected)
                {
                    _hostRdp.Rdp.Disconnect();
                }
                _hostRdp.Invoke((MethodInvoker)delegate { _hostRdp.Dispose(); });
                _hostRdp = null;
            }

            UI = null;
        }

        #region RDP
        
        private void Host_SizeChanged_Initial(object sender, SizeChangedEventArgs e)
        {
            PrepareSessionDisplaSettingsAndConnect();
            _hostGrid.SizeChanged -= Host_SizeChanged_Initial;
        }

        private void Rdp_OnDisconnected(object sender, DisconnectReason reason)
        {
            IsConnected = false;
            Disconnected?.Invoke(this, reason);
        }

        private void PrepareSessionDisplaSettingsAndConnect()
        {
            var size = GetClientSize();
            _hostRdp.Rdp.DesktopWidth = (int)size.Width;
            _hostRdp.Rdp.DesktopHeight = (int)size.Height;
            try
            {
                _hostRdp.Rdp.Connect();
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
                if (_hostRdp.Rdp.Connected)
                {
                    try
                    {
                        var size = GetClientSize();
                        _hostRdp.Rdp.UpdateSessionDisplaySettings((int)size.Width, (int)size.Height);
                    }
                    catch
                    { }
                }
            }
            WindowsInterop.SetWindowPos(
                _topWindowHandle, IntPtr.Zero,
                0, 0, 0, 0, WindowsInterop.SWP_NOSIZE);
        }

        #endregion RDP

        #region Commands

        private void Reconnect()
        {
            Disconnect();
            Connect();
        }

        private void ToggleFullscreen()
        {
            _hostRdp.Rdp.FullScreen = !_hostRdp.Rdp.FullScreen;
            if (_hostRdp.Rdp.Connected)
            {
                try
                {
                    var size = GetClientSize();
                    _hostRdp.Rdp.UpdateSessionDisplaySettings((int)size.Width, (int)size.Height);
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
            if (_hostRdp.Rdp.FullScreen)
            {
                var gridLocation = _hostGrid.PointToScreen(new Point(0, 0));
                var screen = Screen.FromPoint(new System.Drawing.Point((int)gridLocation.X, (int)gridLocation.Y));

                return new Size(screen.Bounds.Width, screen.Bounds.Height);
            }

            return new Size(_hostRdp.Width, _hostRdp.Height);
        }

        private void SendCtrlAltDel()
        {
            if (_hostRdp.Rdp.Connected)
            {
                _hostRdp.Rdp.SendCtrlAltDel();
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
