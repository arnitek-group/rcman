using GalaSoft.MvvmLight;
using RemoteConnectionManager.Core;
using System.Linq;

namespace RemoteConnectionManager.ViewModels
{
    public class SelectionViewModel: ViewModelBase
    {
        public object SelectedPane
        {
            get { return _selectedPane; }
            set
            {
                if (_selectedPane != value)
                {
                    _selectedPane = value;
                    RaisePropertyChanged();

                    if (value is IConnection)
                    {
                        SelectedConnection = (IConnection) value;
                    }
                }
            }
        }

        private object _selectedObject;
        public object SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                if (_selectedObject != value)
                {
                    _selectedObject = value;
                    RaisePropertyChanged();

                    SelectedConnectionSettings = value as ConnectionSettingsViewModel;
                    SelectedCredentials = value as CredentialsViewModel;
                }
            }
        }

        private CredentialsViewModel _selectedCredentials;
        public CredentialsViewModel SelectedCredentials
        {
            get { return _selectedCredentials; }
            set
            {
                if (_selectedCredentials != value)
                {
                    _selectedCredentials = value;
                    RaisePropertyChanged();

                    ViewModelLocator.Locator.Selection.SelectedObject = value;
                }
                ViewModelLocator.Locator.Settings.DeleteCredentialsCommand.RaiseCanExecuteChanged();
            }
        }

        private ConnectionSettingsViewModel _selectedConnectionSettings;
        public ConnectionSettingsViewModel SelectedConnectionSettings
        {
            get { return _selectedConnectionSettings; }
            set
            {
                if (_selectedConnectionSettings != value)
                {
                    _selectedConnectionSettings = value;
                    RaisePropertyChanged();

                    if (_selectedConnectionSettings != null)
                    {
                        var connection = ViewModelLocator.Locator
                            .Connections.Connections
                            .FirstOrDefault(x => x.ConnectionSettings == _selectedConnectionSettings.ConnectionSettings);
                        if (connection != null)
                        {
                            SelectedConnection = connection;
                        }
                    }
                    ViewModelLocator.Locator.Selection.SelectedObject = value;
                }
                ViewModelLocator.Locator.Settings.DeleteConnectionSettingsCommand.RaiseCanExecuteChanged();
            }
        }

        private IConnection _selectedConnection;
        private object _selectedPane;

        public IConnection SelectedConnection
        {
            get { return _selectedConnection; }
            set
            {
                if (_selectedConnection != value)
                {
                    _selectedConnection = value;
                    RaisePropertyChanged();

                    if (_selectedConnection != null)
                    {
                        var csvm = ViewModelLocator.Locator
                            .Settings.ConnectionSettings
                            .FirstOrDefault(x => x.ConnectionSettings == _selectedConnection.ConnectionSettings);
                        if (csvm != null)
                        {
                            SelectedConnectionSettings = csvm;
                        }

                        SelectedPane = _selectedConnection;
                    }
                }
            }
        }
    }
}
