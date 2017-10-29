using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.ExternalProcess;
using RemoteConnectionManager.Rdp;
using RemoteConnectionManager.Services;

namespace RemoteConnectionManager.ViewModels
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            // Register factories.
            SimpleIoc.Default.Register(() => new IConnectionFactory[]
            {
                new RdpConnectionFactory(),
                new PuTTYConnectionFactory()
            });
            // Register services.
            SimpleIoc.Default.Register<ISettingsService, JsonSettingsService>();
            SimpleIoc.Default.Register<IDialogService, MessageBoxDialogService>();
            // Register view models.
            SimpleIoc.Default.Register<ViewModelLocator>(() => this);
            SimpleIoc.Default.Register<ConnectionsViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();

            Connections = ServiceLocator.Current.GetInstance<ConnectionsViewModel>();
            Settings = ServiceLocator.Current.GetInstance<SettingsViewModel>();
            Locator = ServiceLocator.Current.GetInstance<ViewModelLocator>();
        }

        public ConnectionsViewModel Connections { get; }
        public SettingsViewModel Settings { get; }
        public static ViewModelLocator Locator { get; private set; }
    }
}