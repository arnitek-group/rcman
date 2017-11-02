using GalaSoft.MvvmLight;

namespace RemoteConnectionManager.ViewModels.Properties
{
    public class GenericPropertiesViewModel: ViewModelBase
    {
        public GenericPropertiesViewModel(CategoryItemViewModel parent)
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
    }
}
