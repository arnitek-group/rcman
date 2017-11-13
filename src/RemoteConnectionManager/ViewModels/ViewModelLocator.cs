using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Core.Services;
using RemoteConnectionManager.ExternalProcess;
using RemoteConnectionManager.Rdp;
using RemoteConnectionManager.Services;

namespace RemoteConnectionManager.ViewModels
{
    public class ViewModelLocator: ViewModelBase
    {
        public ViewModelLocator()
        {
            if (!IsInDesignMode)
            {
                ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
                // Register services.
                SimpleIoc.Default.Register<ISettingsService, JsonSettingsService>();
                SimpleIoc.Default.Register<IDialogService, MessageBoxDialogService>();
                SimpleIoc.Default.Register<ITelemetryService, ApplicationInsightsTelemetryService>();
                // Register factories.
                SimpleIoc.Default.Register(() => new IConnectionFactory[]
                {
                new RdpConnectionFactory(ServiceLocator.Current.GetInstance<ITelemetryService>()),
                new PuTTYConnectionFactory()
                });
                // Register view models.
                SimpleIoc.Default.Register<ViewModelLocator>(() => this);
                SimpleIoc.Default.Register<ConnectionsViewModel>();
                SimpleIoc.Default.Register<MainViewModel>();
                SimpleIoc.Default.Register<DragDropViewModel>();
                SimpleIoc.Default.Register<DockViewModel>();
                SimpleIoc.Default.Register<ImportViewModel>();

                TelemetryService = ServiceLocator.Current.GetInstance<ITelemetryService>();
                SettingsService = ServiceLocator.Current.GetInstance<ISettingsService>();

                Connections = ServiceLocator.Current.GetInstance<ConnectionsViewModel>();
                Main = ServiceLocator.Current.GetInstance<MainViewModel>();
                DragDrop = ServiceLocator.Current.GetInstance<DragDropViewModel>();
                Dock = ServiceLocator.Current.GetInstance<DockViewModel>();
                Import = ServiceLocator.Current.GetInstance<ImportViewModel>();

                Locator = ServiceLocator.Current.GetInstance<ViewModelLocator>();
            }
        }

        public ISettingsService SettingsService { get; }
        public ITelemetryService TelemetryService { get; }

        public ConnectionsViewModel Connections { get; }
        public MainViewModel Main { get; }
        public DragDropViewModel DragDrop { get; }
        public DockViewModel Dock { get; }
        public ImportViewModel Import { get; }

        public static ViewModelLocator Locator { get; private set; }
    }
}