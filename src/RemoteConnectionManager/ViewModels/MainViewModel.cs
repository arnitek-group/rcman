using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.Properties;
using System;
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

        public ObservableCollection<IConnection> Connections { get; }

        #region Selection

        private Credentials _selectedCredentials;
        public Credentials SelectedCredentials
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
            SelectedCredentials = credentials;
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
                connection.Terminated += Connection_Terminated;
                Connections.Add(connection);
                connection.Connect();
            }
            SelectedConnection = connection;
        }

        public RelayCommand<IConnection> DisconnectCommand { get; }
        public void ExecuteDisconnectCommand(IConnection connection)
        {
            connection.Terminated -= Connection_Terminated;
            connection.Disconnect();
            Connections.Remove(connection);
        }

        private void Connection_Terminated(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => ExecuteDisconnectCommand((IConnection)sender));
        }

        #endregion
    }
}