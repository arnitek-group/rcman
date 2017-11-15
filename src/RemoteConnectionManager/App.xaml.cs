using RemoteConnectionManager.ViewModels;
using System;
using System.Windows;

namespace RemoteConnectionManager
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += (sender, e) =>
            {
                ViewModelLocator.Locator.TelemetryService.TrackException(e.Exception);
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                if (e.ExceptionObject is Exception exc)
                {
                    ViewModelLocator.Locator.TelemetryService.TrackException(exc);
                }
            };
        }
    }
}
