using GalaSoft.MvvmLight;
using RemoteConnectionManager.Models;
using RemoteConnectionManager.ViewModels.Properties;
using System.Collections.ObjectModel;

namespace RemoteConnectionManager.ViewModels
{
    public class CategoryItemViewModel: ViewModelBase
    {
        public CategoryItemViewModel(CategoryItem categoryItem)
        {
            CategoryItem = categoryItem;

            if (CategoryItem.ConnectionSettings != null)
            {
                ConnectionSettings = new ConnectionSettingsViewModel(this);
                Properties = ConnectionSettings;
            }
            else if (CategoryItem.Credentials != null)
            {
                Credentials =new CredentialsViewModel(this);
                Properties = Credentials;
            }

            Items = new ObservableCollection<CategoryItemViewModel>();
        }

        public CategoryItem CategoryItem { get; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string DisplayName
        {
            get { return CategoryItem.DisplayName; }
            set
            {
                if (CategoryItem.DisplayName != value)
                {
                    CategoryItem.DisplayName = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ViewModelBase Properties { get; }

        public ConnectionSettingsViewModel ConnectionSettings { get; }
        public CredentialsViewModel Credentials { get; }

        public ObservableCollection<CategoryItemViewModel> Items { get; }
    }
}
