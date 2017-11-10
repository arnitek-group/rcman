using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Core.Services;
using RemoteConnectionManager.Properties;
using RemoteConnectionManager.ViewModels.Properties;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using RemoteConnectionManager.Services;

namespace RemoteConnectionManager.ViewModels
{
    public class ConnectionsViewModel : ViewModelBase
    {
        private readonly IConnectionFactory[] _connectionFactories;
        private readonly ITelemetryService _telemetryService;
        private readonly IDialogService _dialogService;

        public ConnectionsViewModel(
            IConnectionFactory[] connectionFactories,
            ITelemetryService telemetryService,
            IDialogService dialogService)
        {
            _connectionFactories = connectionFactories;
            _telemetryService = telemetryService;
            _dialogService = dialogService;

            Protocols = _connectionFactories
                .SelectMany(x => x.Protocols)
                .Distinct()
                .Select(x => new ProtocolViewModel(x));

            Connections = new ObservableCollection<IConnection>();

            ConnectCommand = new RelayCommand<ConnectionSettings>(ExecuteConnectCommand);
            DisconnectCommand = new RelayCommand<IConnection>(ExecuteDisconnectCommand);
        }

        public IEnumerable<ProtocolViewModel> Protocols { get; }

        public bool OnClosing()
        {
            if (Connections.Count > 0)
            {
                if (!_dialogService.ShowConfirmationDialog(Resources.ConfirmClose))
                {
                    return false;
                }
            }

            foreach (var connection in Connections)
            {
                Disconnect(connection, DisconnectReason.ApplicationExit);
            }

            return true;
        }

        public ObservableCollection<IConnection> Connections { get; }

        public RelayCommand<ConnectionSettings> ConnectCommand { get; }
        public void ExecuteConnectCommand(ConnectionSettings connectionSettings)
        {
            if (string.IsNullOrEmpty(connectionSettings.Server))
            {
                _dialogService.ShowWarningDialog(Resources.Error_Server);
                return;
            }

            var connection = Connections.FirstOrDefault(x => x.ConnectionSettings == connectionSettings);
            if (connection == null)
            {
                connection = _connectionFactories
                    .First(x => x.Protocols.Contains(connectionSettings.Protocol))
                    .CreateConnection(connectionSettings, ViewModelLocator.Locator.Dock.AutoHideHandle);
                Connections.Add(connection);
            }
            if (!connection.IsConnected)
            {
                connection.Disconnected += ConnectionDisconnected;
                connection.Connect();

                _telemetryService.TrackEvent(
                    "Connect",
                    new Dictionary<string, string>
                    {
                        {"Protocol", connection.ConnectionSettings.Protocol.ToString().ToUpper()}
                    });
            }

            ViewModelLocator.Locator.Dock.ActiveContent = connection;
        }

        public RelayCommand<IConnection> DisconnectCommand { get; }
        public void ExecuteDisconnectCommand(IConnection connection)
        {
            Disconnect(connection, DisconnectReason.ConnectionEnded);
        }

        private void ConnectionDisconnected(object sender, DisconnectReason e)
        {
            Application.Current.Dispatcher.Invoke(() => Disconnect((IConnection)sender, e));
        }

        private void Disconnect(IConnection connection, DisconnectReason reason)
        {
            connection.Disconnected -= ConnectionDisconnected;
            connection.Disconnect();

            // The user initiated the disconnect so we
            // can remove the connection.
            if (reason == DisconnectReason.ApplicationExit ||
                reason == DisconnectReason.ConnectionEnded)
            {
                connection.Destroy();
            }
            if (reason == DisconnectReason.ConnectionEnded)
            {
                Connections.Remove(connection);
            }
        }
    }
}