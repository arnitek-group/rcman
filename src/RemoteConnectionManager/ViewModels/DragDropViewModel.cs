using GalaSoft.MvvmLight;
using RemoteConnectionManager.Extensions;
using System;
using System.Linq;

namespace RemoteConnectionManager.ViewModels
{
    public class DragDropViewModel : ViewModelBase
    {
        public void Add(CategoryItemViewModel dragSource, CategoryItemViewModel dropTarget)
        {
            HandleDrop(dragSource, dropTarget, () =>
            {
                if (dropTarget != null)
                {
                    dropTarget.CategoryItem.Items.Add(dragSource.CategoryItem);
                    dropTarget.Items.Add(dragSource);

                    dragSource.Parent = dropTarget;
                }
                else
                {
                    ViewModelLocator.Locator.Main.Items.Add(dragSource);
                }
            });
        }

        public void InsertBefore(CategoryItemViewModel dragSource, CategoryItemViewModel dropTarget)
        {
            Insert(dragSource, dropTarget, -1);
        }

        public void InsertAfter(CategoryItemViewModel dragSource, CategoryItemViewModel dropTarget)
        {
            Insert(dragSource, dropTarget, +1);
        }

        private void Insert(CategoryItemViewModel dragSource, CategoryItemViewModel dropTarget, int indexModifier)
        {
            HandleDrop(dragSource, dropTarget, () =>
            {
                if (dropTarget != null && dropTarget.Parent != null)
                {
                    var parent = dropTarget.Parent;
                    var index = parent.CategoryItem.Items.IndexOf(dropTarget.CategoryItem) + indexModifier;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    else if (index > parent.CategoryItem.Items.Count)
                    {
                        index = parent.CategoryItem.Items.Count;
                    }

                    parent.CategoryItem.Items.Insert(index, dragSource.CategoryItem);
                    parent.Items.Insert(index, dragSource);

                    dragSource.Parent = parent;
                }
                else
                {
                    ViewModelLocator.Locator.Main.Items.Add(dragSource);
                }
            });
        }

        private void HandleDrop(
            CategoryItemViewModel dragSource,
            CategoryItemViewModel dropTarget,
            Action doDrop)
        {
            if (dragSource == null)
            {
                return;
            }

            // Prevent dropping a parent on its child.
            if (dropTarget != null)
            {
                if (dragSource.Items.GetFlatList(x => x.Items, x => x == dropTarget).Any())
                {
                    return;
                }
            }

            ViewModelLocator.Locator.Main.SuspendSave = true;

            // Remove the drag source from it's parent.
            if (dragSource.Parent != null)
            {
                dragSource.Parent.CategoryItem.Items.Remove(dragSource.CategoryItem);
                dragSource.Parent.Items.Remove(dragSource);
                dragSource.Parent = null;
            }
            else
            {
                ViewModelLocator.Locator.Main.Items.Remove(dragSource);
            }

            doDrop();

            ViewModelLocator.Locator.Main.SuspendSave = false;
            ViewModelLocator.Locator.Main.SaveConnections();

            ViewModelLocator.Locator.Main.SelectedItem = dragSource;
        }
    }
}
