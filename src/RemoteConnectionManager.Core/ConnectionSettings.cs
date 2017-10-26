using System.ComponentModel;

namespace RemoteConnectionManager.Core
{
    public class ConnectionSettings: Model
    {
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Protocol _protocol;
        public Protocol Protocol
        {
            get { return _protocol; }
            set
            {
                if (_protocol != value)
                {
                    _protocol = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _server;
        public string Server
        {
            get { return _server; }
            set
            {
                if (_server != value)
                {
                    _server = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _port;
        public string Port
        {
            get { return _port; }
            set
            {
                if (_port != value)
                {
                    _port = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Credentials _credentials;
        public Credentials Credentials
        {
            get { return _credentials; }
            set
            {
                if (_credentials != value)
                {
                    _credentials = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
