using RemoteConnectionManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace RemoteConnectionManager.Controls
{
    public class CategoryTreeView : TreeView
    {
        public CategoryTreeView() : base()
        {
            IsHitTestVisible = true;
            AllowDrop = true;
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CategoryTreeViewItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CategoryTreeViewItem();
        }

        protected override void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CategoryItemViewModel)))
            {
                var dragSource = (CategoryItemViewModel)e.Data.GetData(typeof(CategoryItemViewModel));
                ViewModelLocator.Locator.DragDrop.Add(dragSource, null);
            }

            e.Handled = true;
        }
    }
}
