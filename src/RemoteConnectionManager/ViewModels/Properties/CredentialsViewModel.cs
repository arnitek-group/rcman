using GalaSoft.MvvmLight;

namespace RemoteConnectionManager.ViewModels.Properties
{
    public class CredentialsViewModel: ViewModelBase
    {
        public CredentialsViewModel(CategoryItemViewModel parent)
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

        public string Notes
        {
            get { return Parent.Notes; }
            set
            {
                if (Parent.Notes != value)
                {
                    Parent.Notes = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Domain
        {
            get { return Parent.CategoryItem.Credentials.Domain; }
            set
            {
                if (Parent.CategoryItem.Credentials.Domain != value)
                {
                    Parent.CategoryItem.Credentials.Domain = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string Username
        {
            get { return Parent.CategoryItem.Credentials.Username; }
            set
            {
                if (Parent.CategoryItem.Credentials.Username != value)
                {
                    Parent.CategoryItem.Credentials.Username = value;
                    RaisePropertyChanged();
                }
            }
        }
        
        public string Password
        {
            get { return Parent.CategoryItem.Credentials.Password; }
            set
            {
                if (Parent.CategoryItem.Credentials.Password != value)
                {
                    Parent.CategoryItem.Credentials.SetPassword(value);
                    RaisePropertyChanged();
                }
            }
        }

        public string KeyFile
        {
            get { return Parent.CategoryItem.Credentials.KeyFile; }
            set
            {
                if (Parent.CategoryItem.Credentials.KeyFile != value)
                {
                    Parent.CategoryItem.Credentials.KeyFile = value;
                    RaisePropertyChanged();
                }
            }
        }
    }
}
