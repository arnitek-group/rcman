using RemoteConnectionManager.ViewModels;
using RemoteConnectionManager.ViewModels.Properties;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RemoteConnectionManager.Controls
{
    public class CategoryTreeViewItem : TreeViewItem
    {
        public CategoryTreeViewItem() : base()
        {
            IsHitTestVisible = true;
            AllowDrop = true;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CategoryTreeViewItem();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Mouse.LeftButton == MouseButtonState.Pressed && DataContext != null)
            {
                DragDrop.DoDragDrop(this, DataContext, DragDropEffects.Move);
            }
        }

        protected override void OnDrop(DragEventArgs e)
        {
            var dropTarget = DataContext as CategoryItemViewModel;
            if (e.Data.GetDataPresent(typeof(CategoryItemViewModel)))
            {
                var dragSource = (CategoryItemViewModel)e.Data.GetData(typeof(CategoryItemViewModel));
                if (dragSource != dropTarget && dropTarget.Properties is GenericPropertiesViewModel)
                {
                    ViewModelLocator.Locator.DragDrop.Drop(dragSource, dropTarget);
                }
            }

            e.Handled = true;
        }
    }
}
