using System.IO;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace RemoteConnectionManager.Extensions
{
    public static class DockingManagerExtensions
    {
        public static void SaveLayout(this DockingManager dockingManager, string layoutFilePath)
        {
            try
            {
                var serializer = new XmlLayoutSerializer(dockingManager);
                serializer.Serialize(layoutFilePath);
            }
            catch
            { }
        }

        public static void LoadLayout(this DockingManager dockingManager, string layoutFilePath)
        {
            try
            {
                if (File.Exists(layoutFilePath))
                {
                    var serializer = new XmlLayoutSerializer(dockingManager);
                    serializer.Deserialize(layoutFilePath);
                }
            }
            catch
            { }
        }
    }
}
