using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RemoteConnectionManager.ViewModels
{
    public class ConnectionsViewModel : ViewModelBase
    {
        private readonly IConnectionFactory[] _connectionFactories;

        public ConnectionsViewModel(IConnectionFactory[] connectionFactories)
        {
            _connectionFactories = connectionFactories;

            Connections = new ObservableCollection<IConnection>();

            ConnectCommand = new RelayCommand<ConnectionSettings>(ExecuteConnectCommand);
            DisconnectCommand = new RelayCommand<IConnection>(ExecuteDisconnectCommand);
        }

        public bool OnClosing()
        {
            foreach (var connection in Connections)
            {
                connection.Disconnect();
            }

            return true;
        }
        
        public ObservableCollection<IConnection> Connections { get; }
        
        private CredentialsViewModel _selectedCredentials;
        public CredentialsViewModel SelectedCredentials
        {
            get { return _selectedCredentials; }
            set
            {
                if (_selectedCredentials != value)
                {
                    _selectedCredentials = value;
                    RaisePropertyChanged();
                }
                ViewModelLocator.Locator.Settings.DeleteCredentialsCommand.RaiseCanExecuteChanged();
            }
        }

        private ConnectionSettingsViewModel _selectedConnectionSettings;
        public ConnectionSettingsViewModel SelectedConnectionSettings
        {
            get { return _selectedConnectionSettings; }
            set
            {
                if (_selectedConnectionSettings != value)
                {
                    _selectedConnectionSettings = value;
                    RaisePropertyChanged();

                    if (_selectedConnectionSettings != null)
                    {
                        var connection = Connections
                            .FirstOrDefault(x => x.ConnectionSettings == _selectedConnectionSettings.ConnectionSettings);
                        if (connection != null)
                        {
                            SelectedConnection = connection;
                        }
                    }
                }
                ViewModelLocator.Locator.Settings.DeleteConnectionSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        private IConnection _selectedConnection;
        public IConnection SelectedConnection
        {
            get { return _selectedConnection; }
            set
            {
                if (_selectedConnection != value)
                {
                    _selectedConnection = value;
                    RaisePropertyChanged();

                    if (_selectedConnection != null)
                    {
                        var csvm = ViewModelLocator.Locator.Settings.ConnectionSettings
                            .FirstOrDefault(x => x.ConnectionSettings == _selectedConnection.ConnectionSettings);
                        if (csvm != null)
                        {
                            SelectedConnectionSettings = csvm;
                        }
                    }
                }
            }
        }
        
        public RelayCommand<ConnectionSettings> ConnectCommand { get; }
        public void ExecuteConnectCommand(ConnectionSettings connectionSettings)
        {
            var connection = Connections.FirstOrDefault(x => x.ConnectionSettings == connectionSettings);
            if (connection == null)
            {
                connection = _connectionFactories
                    .First(x => x.CanConnect(connectionSettings))
                    .CreateConnection(connectionSettings);
                Connections.Add(connection);
            }
            if (!connection.IsConnected)
            {
                connection.Disconnected += ConnectionDisconnected;
                connection.Connect();
            }
            SelectedConnection = connection;
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

            if (reason == DisconnectReason.ConnectionEnded)
            {
                // The user initiated the disconnect so we
                // can remove the connection.
                Connections.Remove(connection);
                connection.Destroy();
            }
        }
    }
}