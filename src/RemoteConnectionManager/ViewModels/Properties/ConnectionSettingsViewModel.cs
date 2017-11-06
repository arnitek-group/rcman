using GalaSoft.MvvmLight;
using RemoteConnectionManager.Core.Connections;

namespace RemoteConnectionManager.ViewModels.Properties
{
    public class ConnectionSettingsViewModel: ViewModelBase
    {
        public ConnectionSettingsViewModel(CategoryItemViewModel parent)
        {
            Parent = parent;
        }

        public CategoryItemViewModel Parent { get; }

        public string DisplayName
        {
            get { return Parent.DisplayName; }
            set
            {
                if (Parent.DisplayName != value)
                {
                    Parent.DisplayName = value;
                    RaisePropertyChanged();
                }
            }
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
