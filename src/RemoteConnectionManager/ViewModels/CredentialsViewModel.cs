using GalaSoft.MvvmLight;
using RemoteConnectionManager.Core;

namespace RemoteConnectionManager.ViewModels
{
    public class CredentialsViewModel: ViewModelBase
    {
        public CredentialsViewModel(Credentials credentials)
        {
            Credentials = credentials;
        }

        public Credentials Credentials { get; }

        public string Domain
        {
            get { return Credentials.Domain; }
            set
            {
                if (Credentials.Domain != value)
                {
                    Credentials.Domain = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Username
        {
            get { return Credentials.Username; }
            set
            {
                if (Credentials.Username != value)
                {
                    Credentials.Username = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public string Password
        {
            get { return Credentials.Password; }
            set
            {
                if (Credentials.Password != value)
                {
                    Credentials.SetPassword(value);
                    RaisePropertyChanged();
                }
            }
        }
        
        public string DisplayName
        {
            get { return Credentials.DisplayName; }
            set
            {
                if (Credentials.DisplayName != value)
                {
                    Credentials.DisplayName = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
