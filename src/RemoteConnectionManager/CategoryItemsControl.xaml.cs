using RemoteConnectionManager.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace RemoteConnectionManager
{
    public partial class CategoryItemsControl : UserControl
    {
        public CategoryItemsControl()
        {
            InitializeComponent();
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var civm = (CategoryItemViewModel) ((TreeViewItem) sender).DataContext;
            if (civm.CategoryItem.ConnectionSettings != null)
            {
                ViewModelLocator.Locator
                    .Connections
                    .ExecuteConnectCommand(civm.CategoryItem.ConnectionSettings);
            }
        }
    }
}
