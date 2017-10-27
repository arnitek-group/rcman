using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.Properties;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace RemoteConnectionManager.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly SettingsViewModel _settingsViewModel;
        private readonly IConnectionFactory[] _connectionFactories;

        public MainViewModel(SettingsViewModel settingsViewModel, IConnectionFactory[] connectionFactories)
        {
            _settingsViewModel = settingsViewModel;
            _connectionFactories = connectionFactories;

            var credentialsVms = _settingsViewModel.Credentials.Select(x => new CredentialsViewModel(x));
            Credentials = new ObservableCollection<CredentialsViewModel>(credentialsVms);
            Connections = new ObservableCollection<IConnection>();
            
            NewConnectionSettingsCommand = new RelayCommand(ExecuteNewConnectionSettingsCommand);
            NewCredentialsCommand = new RelayCommand(ExecuteNewCredentialsCommand);

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

        public ObservableCollection<CredentialsViewModel> Credentials { get; }
        public ObservableCollection<IConnection> Connections { get; }

        #region Selection

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
            }
        }

        private ConnectionSettings _selectedConnectionSettings;
        public ConnectionSettings SelectedConnectionSettings
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
                            .FirstOrDefault(x => x.ConnectionSettings == _selectedConnectionSettings);
                        if (connection != null)
                        {
                            SelectedConnection = connection;
                        }
                    }
                }
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
                        var connectionSettings = _settingsViewModel.ConnectionSettings
                            .FirstOrDefault(x => x == _selectedConnection.ConnectionSettings);
                        if (connectionSettings != null)
                        {
                            SelectedConnectionSettings = connectionSettings;
                        }
                    }
                }
            }
        }

        #endregion

        #region Commands

        public RelayCommand NewConnectionSettingsCommand { get; }
        public void ExecuteNewConnectionSettingsCommand()
        {
            var connectionSettings = new ConnectionSettings
            {
                DisplayName = Resources.New
            };
            _settingsViewModel.ConnectionSettings.Add(connectionSettings);
            SelectedConnectionSettings = connectionSettings;
        }

        public RelayCommand NewCredentialsCommand { get; }
        public void ExecuteNewCredentialsCommand()
        {
            var credentials = new Credentials
            {
                DisplayName = Resources.New
            };
            _settingsViewModel.Credentials.Add(credentials);

            var credentialsVm = new CredentialsViewModel(credentials);
            Credentials.Add(credentialsVm);
            SelectedCredentials = credentialsVm;
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
                connection.Disconnected += ConnectionDisconnected;
                Connections.Add(connection);
            }
            if (!connection.IsConnected)
            {
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

        #endregion
    }
}