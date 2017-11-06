using RemoteConnectionManager.Core.Connections;
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

            var assembly = Assembly.GetExecutingAssembly();
            var vi = FileVersionInfo.GetVersionInfo(assembly.Location);

            Title = $"{Properties.Resources.Application} v{vi.ProductMajorPart}.{vi.ProductMinorPart}.{vi.ProductBuildPart}";

            Loaded += (sender, e) =>
            {
                ViewModelLocator.Locator.TelemetryService.TrackPage("Application");
            };
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
