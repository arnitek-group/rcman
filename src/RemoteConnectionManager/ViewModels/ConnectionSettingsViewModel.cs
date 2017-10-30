using GalaSoft.MvvmLight;
using RemoteConnectionManager.Core;

namespace RemoteConnectionManager.ViewModels
{
    public class ConnectionSettingsViewModel: ViewModelBase
    {
        public ConnectionSettingsViewModel(ConnectionSettings connectionSettings)
        {
            ConnectionSettings = connectionSettings;
        }

        public ConnectionSettings ConnectionSettings { get; }
        
        public string DisplayName
        {
            get { return ConnectionSettings.DisplayName; }
            set
            {
                if (ConnectionSettings.DisplayName != value)
                {
                    ConnectionSettings.DisplayName = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public Protocol Protocol
        {
            get { return ConnectionSettings.Protocol; }
            set
            {
                if (ConnectionSettings.Protocol != value)
                {
                    ConnectionSettings.Protocol = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public string Server
        {
            get { return ConnectionSettings.Server; }
            set
            {
                if (ConnectionSettings.Server != value)
                {
                    ConnectionSettings.Server = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public string Port
        {
            get { return ConnectionSettings.Port; }
            set
            {
                if (ConnectionSettings.Port != value)
                {
                    ConnectionSettings.Port = value;
                    RaisePropertyChanged();
                }
            }
        }

        private CredentialsViewModel _credentials;
        public CredentialsViewModel Credentials
        {
            get { return _credentials; }
            set
            {
                if (_credentials != value)
                {
                    _credentials = value;
                    ConnectionSettings.Credentials = _credentials?.CredentialsM;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
