using RemoteConnectionManager.Core.Connections;
using RemoteConnectionManager.Extensions;
using RemoteConnectionManager.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock;

namespace RemoteConnectionManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            var vi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            Title = $"{Properties.Resources.Application} v{vi.ProductMajorPart}.{vi.ProductMinorPart}.{vi.ProductBuildPart}";
            
            Loaded += (sender, e) =>
            {
                ViewModelLocator.Locator.TelemetryService.TrackPage("Application");
            };
            DockingManager.Loaded += (sender, e) =>
            {
                DockingManager.LoadLayout(ViewModelLocator.Locator.Settings.LayoutFile);
                DockingManager.AutoHideWindow.Loaded += (sender1, e1) =>
                {
                    ViewModelLocator.Locator.Dock.AutoHideHandle = DockingManager.AutoHideWindow.Handle;
                };
                DockingManager.AutoHideWindow.IsVisibleChanged += (sender1, e1) =>
                {
                    if (DockingManager.AutoHideWindow.IsVisible)
                    {
                        ViewModelLocator.Locator.Connections.Connections.ForEach(x => x.ZIndexFix());
                    }
                };
            };
            DockingManager.DocumentClosed += DockingManager_OnDocumentClosed;
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
                ViewModelLocator.Locator.Settings.SaveSettings();
                DockingManager.SaveLayout(ViewModelLocator.Locator.Settings.LayoutFile);
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
