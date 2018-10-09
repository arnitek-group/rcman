using RemoteConnectionManager.ViewModels;
using RemoteConnectionManager.ViewModels.Properties;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace RemoteConnectionManager.Controls
{
    public class CategoryTreeViewItem : TreeViewItem
    {
        public CategoryTreeViewItem() : base()
        {
            IsHitTestVisible = true;
            AllowDrop = true;

            Loaded += CategoryTreeViewItem_Loaded;
        }

        private void CategoryTreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= CategoryTreeViewItem_Loaded;

            var itemsControl = ItemsControlFromItemContainer(this);

            WeakEventManager<ItemContainerGenerator, ItemsChangedEventArgs>.AddHandler(
                itemsControl.ItemContainerGenerator,
                "ItemsChanged",
                (sender1, e1) => SetExtender());

            SetExtender();
        }

        #region UseExtender

        public bool UseExtender
        {
            get { return (bool)GetValue(UseExtenderProperty); }
            set { SetValue(UseExtenderProperty, value); }
        }

        public static readonly DependencyProperty UseExtenderProperty = DependencyProperty.Register(
            "UseExtender", typeof(bool),
            typeof(CategoryTreeViewItem),
            new PropertyMetadata(false));

        private void SetExtender()
        {
            var itemsControl = ItemsControlFromItemContainer(this);
            if (itemsControl != null)
            {
                SetValue(UseExtenderProperty, itemsControl.ItemContainerGenerator.IndexFromContainer(this) == itemsControl.Items.Count - 1);
            }
        }

        #endregion

        #region Drag and Drop

        private bool IsValidDropTarget(DragEventArgs e, out CategoryItemViewModel dragSource, out CategoryItemViewModel dropTarget)
        {
            dragSource = null;
            dropTarget = DataContext as CategoryItemViewModel;
            if (e.Data.GetDataPresent(typeof(CategoryItemViewModel)))
            {
                dragSource = (CategoryItemViewModel)e.Data.GetData(typeof(CategoryItemViewModel));
                if (dragSource != dropTarget)
                {
                    return true;
                }
            }

            return false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Mouse.LeftButton == MouseButtonState.Pressed && DataContext != null)
            {
                DragDrop.DoDragDrop(this, DataContext, DragDropEffects.Move);
            }
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            CategoryItemViewModel dragSource;
            CategoryItemViewModel dropTarget;
            if (IsValidDropTarget(e, out dragSource, out dropTarget))
            {
                // TODO: Visual Indicator
            }

            base.OnDragEnter(e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            CategoryItemViewModel dragSource;
            CategoryItemViewModel dropTarget;
            if (IsValidDropTarget(e, out dragSource, out dropTarget))
            {
                // TODO: Visual Indicator
            }

            base.OnDragLeave(e);
        }

        protected override void OnDrop(DragEventArgs e)
        {
            CategoryItemViewModel dragSource;
            CategoryItemViewModel dropTarget;
            if (IsValidDropTarget(e, out dragSource, out dropTarget))
            {
                if (dropTarget.Properties is GenericPropertiesViewModel)
                {
                    ViewModelLocator.Locator.DragDrop.Add(dragSource, dropTarget);
                }
                else
                {
                    // TODO
                    // ViewModelLocator.Locator.DragDrop.InsertAfter
                    // ViewModelLocator.Locator.DragDrop.InsertBefore
                }
            }

            e.Handled = true;
        }

        #endregion

        #region Overrides

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CategoryTreeViewItem();
        }

        #endregion
    }
}
