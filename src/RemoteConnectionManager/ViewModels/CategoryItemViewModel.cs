using GalaSoft.MvvmLight;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.ViewModels.Properties;
using System.Collections.ObjectModel;

namespace RemoteConnectionManager.ViewModels
{
    public class CategoryItemViewModel: ViewModelBase
    {
        public CategoryItemViewModel(CategoryItem categoryItem, CategoryItemViewModel parent)
        {
            CategoryItem = categoryItem;
            Parent = parent;

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
            else
            {
                Properties = new GenericPropertiesViewModel(this);
            }

            Items = new ObservableCollection<CategoryItemViewModel>();
        }

        public CategoryItem CategoryItem { get; }
        public CategoryItemViewModel Parent { get; set; }

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

        public string Notes
        {
            get { return CategoryItem.Notes; }
            set
            {
                if (CategoryItem.Notes != value)
                {
                    CategoryItem.Notes = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsExpanded
        {
            get { return CategoryItem.IsExpanded; }
            set
            {
                if (CategoryItem.IsExpanded != value)
                {
                    CategoryItem.IsExpanded = value;
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
