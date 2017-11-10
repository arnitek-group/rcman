using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Core.Interop;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Forms = System.Windows.Forms;

namespace RemoteConnectionManager.ExternalProcess
{
    public abstract class HostedProcessConnection : IConnection
    {
        private readonly IntPtr _topWindowHandle;

        protected HostedProcessConnection(ConnectionSettings connectionSettings, IntPtr topWindowHandle)
        {
            ConnectionSettings = connectionSettings;

            _topWindowHandle = topWindowHandle;
        }

        public ConnectionSettings ConnectionSettings { get; }

        public bool IsConnected { get; private set; }
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
        {
        }

        public FrameworkElement UI { get; private set; }
        public System.Windows.Controls.ContextMenu ContextMenu => null;

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

            WindowsInterop.SetParent(_process.MainWindowHandle, _hostPanel.Handle);
            WindowsInterop.SetWindowPos(
                _process.MainWindowHandle, IntPtr.Zero,
                -FrameSides, -FrameTop,
                _hostPanel.Width + 2 * FrameSides,
                _hostPanel.Height + FrameSides + FrameTop,
                WindowsInterop.SWP_NOACTIVATE | WindowsInterop.SWP_SHOWWINDOW);

            _hostGrid = new Grid();
            _hostGrid.Children.Add(new WindowsFormsHost { Child = _hostPanel });

            Observable
                .FromEventPattern<SizeChangedEventArgs>(_hostGrid, "SizeChanged")
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
            WindowsInterop.SetWindowPos(
                _process.MainWindowHandle, IntPtr.Zero,
                -FrameSides, -FrameTop,
                _hostPanel.Width + 2 * FrameSides,
                _hostPanel.Height + FrameSides + FrameTop,
                WindowsInterop.SWP_NOACTIVATE | WindowsInterop.SWP_SHOWWINDOW);
            WindowsInterop.SetWindowPos(
                _topWindowHandle, IntPtr.Zero,
                0, 0, 0, 0, WindowsInterop.SWP_NOSIZE);
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

        private const int FrameTop = 32;
        private const int FrameSides = 8;

        #endregion
    }
}
