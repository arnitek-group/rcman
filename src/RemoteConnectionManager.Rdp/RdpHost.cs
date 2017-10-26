using RemoteConnectionManager.Core;
using System.Windows.Forms;

namespace RemoteConnectionManager.Rdp
{
    public partial class RdpHost : UserControl
    {
        public RdpHost()
        {
            InitializeComponent();
        }

        public void Connect(ConnectionSettings connectionSettings)
        {
            axMsRdpClient.Server = connectionSettings.Server;

            var credentials = connectionSettings.Credentials;
            if (credentials != null)
            {
                if (!string.IsNullOrEmpty(credentials.Domain))
                {
                    axMsRdpClient.Domain = credentials.Domain;
                }
                if (!string.IsNullOrEmpty(credentials.Username))
                {
                    axMsRdpClient.UserName = credentials.Username;
                }
                if (!string.IsNullOrEmpty(credentials.Password))
                {
                    axMsRdpClient.AdvancedSettings9.ClearTextPassword = credentials.Password;
                }
            }

            axMsRdpClient.AdvancedSettings9.SmartSizing = true;
            axMsRdpClient.Connect();
        }

        public void ResizeClient(int width, int height)
        {
            // TODO: Fix resizing.

            Width = width;
            Height = height;

            axMsRdpClient.Width = width;
            axMsRdpClient.Height = height;
        }

        public void Disconnect()
        {
            if (axMsRdpClient.Connected == 1)
            {
                axMsRdpClient.Disconnect();
            }
        }
    }
}
