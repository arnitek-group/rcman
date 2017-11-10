using GalaSoft.MvvmLight;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Extensions;
using System;
using System.Linq;

namespace RemoteConnectionManager.ViewModels
{
    public class DockViewModel: ViewModelBase
    {
        private object _activeContent;
        public object ActiveContent
        {
            get { return _activeContent; }
            set
            {
                if (_activeContent != value)
                {
                    _activeContent = value;
                    RaisePropertyChanged();

                    if (_activeContent is IConnection connection)
                    {
                        var connectionSettings = ViewModelLocator.Locator
                            .Main.Items
                            .GetFlatList(x => x.Items, x => x.ConnectionSettings != null)
                            .FirstOrDefault(x => x.CategoryItem.ConnectionSettings == connection.ConnectionSettings);
                        if (connectionSettings != null)
                        {
                            ViewModelLocator.Locator
                                .Main.SelectedItem = connectionSettings;
                        }
                    }
                }
            }
        }

        public IntPtr AutoHideHandle { get; set; }
    }
}
