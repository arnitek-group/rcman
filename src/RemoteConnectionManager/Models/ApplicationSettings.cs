using System.Windows;

namespace RemoteConnectionManager.Models
{
    public class ApplicationSettings
    {
        public string Version { get; set; }
        public string LayoutFile { get; set; }
        public string ConnectionsFile { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
        public WindowState WindowState { get; set; }
        public Theme Theme { get; set; }
    }
}
