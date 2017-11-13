using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Models;
using RemoteConnectionManager.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Controls;

namespace RemoteConnectionManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            var settings = ViewModelLocator.Locator.SettingsService.LoadSettings();
            if (settings != null)
            {
                Width = settings.Width;
                Height = settings.Height;
                if (settings.IsMaximized)
                {
                    WindowState = WindowState.Maximized;
                }
            }
            
            var vi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Title = $"{Properties.Resources.Application} v{vi.ProductMajorPart}.{vi.ProductMinorPart}.{vi.ProductBuildPart}";

            Loaded += (sender, e) =>
            {
                ViewModelLocator.Locator.TelemetryService.TrackPage("Application");
            };
        }

        private void DockingManager_Loaded(object sender, RoutedEventArgs e)
        {
            var dock = (DockingManager)sender;
            dock.AutoHideWindow.Loaded += AutoHideWindow_Loaded;
        }

        private void AutoHideWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var control = (LayoutAutoHideWindowControl)sender;
            ViewModelLocator.Locator.Dock.AutoHideHandle = control.Handle;
        }

        private void DockingManager_OnDocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            var connection = (IConnection)e.Document.Content;
            ViewModelLocator.Locator.Connections.ExecuteDisconnectCommand(connection);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!ViewModelLocator.Locator.Connections.OnClosing())
            {
                e.Cancel = true;
            }
            else
            {
                ViewModelLocator.Locator.TelemetryService.TrackPage("Exit");
                ViewModelLocator.Locator.SettingsService.SaveSettings(new ApplicationSettings
                {
                    Width = Width,
                    Height = Height,
                    IsMaximized = WindowState == WindowState.Maximized
                });
            }
        }
    }

    public class ItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ConnectionTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item is DataTemplate template
                ? template
                : ConnectionTemplate;
        }
    }
}
