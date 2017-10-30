using GalaSoft.MvvmLight;
using RemoteConnectionManager.Core;

namespace RemoteConnectionManager.ViewModels
{
    public class CredentialsViewModel: ViewModelBase
    {
        public CredentialsViewModel(Credentials credentials)
        {
            CredentialsM = credentials;
        }
        
        public Credentials CredentialsM { get; }

        public string Domain
        {
            get { return CredentialsM.Domain; }
            set
            {
                if (CredentialsM.Domain != value)
                {
                    CredentialsM.Domain = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Username
        {
            get { return CredentialsM.Username; }
            set
            {
                if (CredentialsM.Username != value)
                {
                    CredentialsM.Username = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public string Password
        {
            get { return CredentialsM.Password; }
            set
            {
                if (CredentialsM.Password != value)
                {
                    CredentialsM.SetPassword(value);
                    RaisePropertyChanged();
                }
            }
        }
        
        public string DisplayName
        {
            get { return CredentialsM.DisplayName; }
            set
            {
                if (CredentialsM.DisplayName != value)
                {
                    CredentialsM.DisplayName = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
