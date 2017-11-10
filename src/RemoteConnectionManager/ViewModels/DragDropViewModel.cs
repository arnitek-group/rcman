using GalaSoft.MvvmLight;
using RemoteConnectionManager.Extensions;
using System.Linq;

namespace RemoteConnectionManager.ViewModels
{
    public class DragDropViewModel : ViewModelBase
    {
        public void Drop(CategoryItemViewModel dragSource, CategoryItemViewModel dropTarget)
        {
            ViewModelLocator.Locator.Main.SuspendSave = true;

            // Prevent dropping a parent on its child.
            if (dropTarget != null)
            {
                if (dragSource.Items.GetFlatList(x => x.Items, x => x == dropTarget).Any())
                {
                    return;
                }
            }

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

            ViewModelLocator.Locator.Main.SuspendSave = false;
            ViewModelLocator.Locator.Main.SaveConnections();

            ViewModelLocator.Locator.Main.SelectedItem = dragSource;
        }
    }
}
