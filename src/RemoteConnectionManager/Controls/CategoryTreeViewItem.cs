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
        public enum DropActionEnum
        {
            None,
            InsertBefore,
            Add,
            InsertAfter
        }

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

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CategoryTreeViewItem();
        }

        private FrameworkElement _headerElement;

        public override void OnApplyTemplate()
        {
            _headerElement = (FrameworkElement)Template.FindName("PART_Header", this);
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

        #region DropAction

        public DropActionEnum DropAction
        {
            get { return (DropActionEnum)GetValue(DropActionProperty); }
            set { SetValue(DropActionProperty, value); }
        }

        public static readonly DependencyProperty DropActionProperty = DependencyProperty.Register(
            "DropAction", typeof(DropActionEnum),
            typeof(CategoryTreeViewItem),
            null);

        #endregion

        #region Drag and Drop

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Mouse.LeftButton == MouseButtonState.Pressed && DataContext != null)
            {
                DragDrop.DoDragDrop(this, DataContext, DragDropEffects.Move);
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            CategoryItemViewModel dragSource;
            CategoryItemViewModel dropTarget;
            if (IsValidDropTarget(e, out dragSource, out dropTarget))
            {
                DropAction = GetDropAction(e, dropTarget);
            }

            e.Handled = true;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            DropAction = DropActionEnum.None;

            e.Handled = true;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            CategoryItemViewModel dragSource;
            CategoryItemViewModel dropTarget;
            if (IsValidDropTarget(e, out dragSource, out dropTarget))
            {
                switch (GetDropAction(e, dropTarget))
                {
                    case DropActionEnum.InsertBefore:
                        ViewModelLocator.Locator.DragDrop.InsertBefore(dragSource, dropTarget);
                        break;
                    case DropActionEnum.Add:
                        ViewModelLocator.Locator.DragDrop.Add(dragSource, dropTarget);
                        break;
                    case DropActionEnum.InsertAfter:
                        ViewModelLocator.Locator.DragDrop.InsertAfter(dragSource, dropTarget);
                        break;
                }
            }

            DropAction = DropActionEnum.None;

            e.Handled = true;
        }

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

        private DropActionEnum GetDropAction(DragEventArgs e, CategoryItemViewModel dropTarget)
        {
            var delta = e.GetPosition(this).Y / _headerElement.ActualHeight * 100;

            if (dropTarget.Properties is GenericPropertiesViewModel)
            {
                if (delta < 27) return DropActionEnum.InsertBefore;
                if (delta < 75) return DropActionEnum.Add;
                return DropActionEnum.InsertAfter;
            }

            if (delta < 50) return DropActionEnum.InsertBefore;

            return DropActionEnum.InsertAfter;
        }

        #endregion
    }
}
