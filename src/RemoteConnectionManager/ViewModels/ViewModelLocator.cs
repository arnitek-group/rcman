using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RemoteConnectionManager.Core;
using RemoteConnectionManager.ExternalProcess;
using RemoteConnectionManager.Rdp;

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
            // Register view models.
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();

            // Trick to create the settings view model observer.
            var settings = Settings;
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();
    }
}