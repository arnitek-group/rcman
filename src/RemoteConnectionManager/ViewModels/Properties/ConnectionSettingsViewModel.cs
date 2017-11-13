using RemoteConnectionManager.Core.Connections;

namespace RemoteConnectionManager.ViewModels.Properties
{
    public class ConnectionSettingsViewModel: CredentialsViewModel
    {
        public ConnectionSettingsViewModel(CategoryItemViewModel parent): base(parent)
        {
        }

        public Protocol Protocol
        {
            get { return Parent.CategoryItem.ConnectionSettings.Protocol; }
            set
            {
                if (Parent.CategoryItem.ConnectionSettings.Protocol != value)
                {
                    Parent.CategoryItem.ConnectionSettings.Protocol = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public string Server
        {
            get { return Parent.CategoryItem.ConnectionSettings.Server; }
            set
            {
                if (Parent.CategoryItem.ConnectionSettings.Server != value)
                {
                    Parent.CategoryItem.ConnectionSettings.Server = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public string Port
        {
            get { return Parent.CategoryItem.ConnectionSettings.Port; }
            set
            {
                if (Parent.CategoryItem.ConnectionSettings.Port != value)
                {
                    Parent.CategoryItem.ConnectionSettings.Port = value;
                    RaisePropertyChanged();
                }
            }
        }
        public new string Domain
        {
            get { return Parent.CategoryItem.ConnectionSettings.Domain; }
            set
            {
                if (Parent.CategoryItem.ConnectionSettings.Domain != value)
                {
                    Parent.CategoryItem.ConnectionSettings.Domain = value;
                    RaisePropertyChanged();
                }
            }
        }

        public new string Username
        {
            get { return Parent.CategoryItem.ConnectionSettings.Username; }
            set
            {
                if (Parent.CategoryItem.ConnectionSettings.Username != value)
                {
                    Parent.CategoryItem.ConnectionSettings.Username = value;
                    RaisePropertyChanged();
                }
            }
        }

        public new string Password
        {
            get { return Parent.CategoryItem.ConnectionSettings.Password; }
            set
            {
                if (Parent.CategoryItem.ConnectionSettings.Password != value)
                {
                    Parent.CategoryItem.ConnectionSettings.SetPassword(value);
                    RaisePropertyChanged();
                }
            }
        }

        public new string KeyFile
        {
            get { return Parent.CategoryItem.ConnectionSettings.KeyFile; }
            set
            {
                if (Parent.CategoryItem.ConnectionSettings.KeyFile != value)
                {
                    Parent.CategoryItem.ConnectionSettings.KeyFile = value;
                    RaisePropertyChanged();
                }
            }
        }

        private CredentialsViewModel _credentialsViewModel;
        public CredentialsViewModel Credentials
        {
            get { return _credentialsViewModel; }
            set
            {
                if (_credentialsViewModel != value)
                {
                    _credentialsViewModel = value;
                    Parent.CategoryItem.ConnectionSettings.Credentials = 
                        _credentialsViewModel?.Parent.CategoryItem.Credentials;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
