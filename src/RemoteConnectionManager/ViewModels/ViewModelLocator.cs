using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Core.Services;
using RemoteConnectionManager.ExternalProcess;
using RemoteConnectionManager.Rdp;

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
                SimpleIoc.Default.Register<SettingsViewModel>();
                SimpleIoc.Default.Register<DragDropViewModel>();
                SimpleIoc.Default.Register<DockViewModel>();

                TelemetryService = ServiceLocator.Current.GetInstance<ITelemetryService>();
                Connections = ServiceLocator.Current.GetInstance<ConnectionsViewModel>();
                Settings = ServiceLocator.Current.GetInstance<SettingsViewModel>();
                DragDrop = ServiceLocator.Current.GetInstance<DragDropViewModel>();
                Dock = ServiceLocator.Current.GetInstance<DockViewModel>();
                Locator = ServiceLocator.Current.GetInstance<ViewModelLocator>();
            }
        }

        public ITelemetryService TelemetryService { get; }

        public ConnectionsViewModel Connections { get; }
        public SettingsViewModel Settings { get; }
        public DragDropViewModel DragDrop { get; }
        public DockViewModel Dock { get; }

        public static ViewModelLocator Locator { get; private set; }
    }
}