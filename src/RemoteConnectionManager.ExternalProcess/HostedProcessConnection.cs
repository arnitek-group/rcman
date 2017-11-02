using RemoteConnectionManager.Core;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace RemoteConnectionManager.ExternalProcess
{
    public abstract class HostedProcessConnection : IConnection
    {
        protected HostedProcessConnection(ConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;
        }

        public bool IsConnected { get; private set; }
        public FrameworkElement UI { get; private set; }
        public ConnectionSettings ConnectionSettings { get; }

        public void Connect()
        {
            if (!IsConnected)
            {
                UI = CreateHostedProcess();
                IsConnected = true;
            }
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                DestroyHostedProcess();
                UI = null;
                IsConnected = false;
            }
        }

        public void Destroy()
        { }

        public event EventHandler<DisconnectReason> Disconnected;

        #region Process

        protected abstract string GetFileName();
        protected abstract string GetArguments();

        private Forms.Panel _hostPanel;
        private Grid _hostGrid;
        private Process _process;

        private FrameworkElement CreateHostedProcess()
        {
            _hostPanel = new Forms.Panel();
            _hostPanel.Dock = DockStyle.Fill;
            _hostPanel.BorderStyle = BorderStyle.None;
            _hostPanel.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right |
                AnchorStyles.Bottom |
                AnchorStyles.Left;

            var psi = new ProcessStartInfo();
            psi.FileName = GetFileName();
            psi.Arguments = GetArguments();
            psi.WindowStyle = ProcessWindowStyle.Maximized;
            psi.CreateNoWindow = false;

            _process = Process.Start(psi);
            _process.EnableRaisingEvents = true;
            _process.Exited += Process_Exited;
            _process.Disposed += Process_Disposed;
            _process.WaitForInputIdle();

            SetParent(_process.MainWindowHandle, _hostPanel.Handle);
            SetWindowLong(_process.MainWindowHandle, GWL_STYLE, WS_VISIBLE + WS_MAXIMIZE);
            MoveWindow(_process.MainWindowHandle, 0, 0, _hostPanel.Width, _hostPanel.Height, true);

            _hostGrid = new Grid();
            _hostGrid.Children.Add(new WindowsFormsHost { Child = _hostPanel });

            Observable
                .FromEventPattern<SizeChangedEventArgs>(_hostGrid, "SizeChanged")
                .Throttle(TimeSpan.FromSeconds(1))
                .Subscribe(x => _hostGrid?.Dispatcher.Invoke(Host_SizeChanged));

            return _hostGrid;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Disconnect();
            Disconnected?.Invoke(this, DisconnectReason.ConnectionEnded);
        }

        private void Process_Disposed(object sender, EventArgs e)
        {
            Disconnect();
            Disconnected?.Invoke(this, DisconnectReason.ConnectionEnded);
        }

        private void Host_SizeChanged()
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                _hostPanel.Visible = true;
                // TODO: Improve resizing.
                MoveWindow(_process.MainWindowHandle, 0, 0, _hostPanel.Width, _hostPanel.Height, true);
            }
            else
            {
                _hostPanel.Visible = false;
            }
        }

        private void DestroyHostedProcess()
        {
            if (_process != null && !_process.HasExited)
            {
                _process.Exited -= Process_Exited;
                _process.Disposed -= Process_Disposed;
                _process.Kill();
                _process.Dispose();
                _process = null;
            }

            if (_hostGrid != null)
            {
                _hostGrid.Dispatcher.Invoke(() => _hostGrid.Children.Clear());
                _hostGrid = null;
            }

            if (_hostPanel != null)
            {
                _hostPanel.Invoke((MethodInvoker)delegate { _hostPanel.Dispose(); });
                _hostPanel = null;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        private const int GWL_STYLE = -16;
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_MAXIMIZE = 0x01000000;

        #endregion
    }
}
