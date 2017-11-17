using System.Windows.Forms;

namespace RemoteConnectionManager.Rdp.Clients
{
    internal class RdpClient9 : GenericRdpClient<AxMSTSCLib.AxMsRdpClient9NotSafeForScripting>
    {
        public override AxHost Client { get { return RdpClient; } }

        public override bool Connected
        {
            get { return RdpClient.Connected == 1; }
        }

        public override void Connect()
        {
            RdpClient.AdvancedSettings2.SmartSizing = true;
            //_hostRdp.AxMsRdpClient.AdvancedSettings4.ConnectionBarShowMinimizeButton = false;
            // Keyboard redirection settings.
            // https://msdn.microsoft.com/en-us/library/aa381095(v=vs.85).aspx
            // https://msdn.microsoft.com/en-us/library/aa381299(v=vs.85).aspx
            RdpClient.SecuredSettings2.KeyboardHookMode = 1;
            RdpClient.AdvancedSettings.allowBackgroundInput = 1;
            RdpClient.AdvancedSettings2.EnableWindowsKey = 1;
            RdpClient.AdvancedSettings7.EnableCredSspSupport = true;

            RdpClient.OnDisconnected += AxMsRdpClient_OnDisconnected;
            RdpClient.Connect();
        }

        public override void Disconnect()
        {
            RdpClient.OnDisconnected -= AxMsRdpClient_OnDisconnected;
            RdpClient.Disconnect();
        }

        public override string Server
        {
            get { return RdpClient.Server; }
            set
            {
                RdpClient.Server = value;
                RdpClient.FullScreenTitle = value;
            }
        }

        public override int Port
        {
            get { return RdpClient.AdvancedSettings2.RDPPort; }
            set { RdpClient.AdvancedSettings2.RDPPort = value; }
        }

        public override string Domain
        {
            get { return RdpClient.Domain; }
            set { RdpClient.Domain = value; }
        }

        public override string UserName
        {
            get { return RdpClient.UserName; }
            set { RdpClient.UserName = value; }
        }

        public override string Password
        {
            set { RdpClient.AdvancedSettings2.ClearTextPassword = value; }
        }

        public override string ConnectingText
        {
            get { return RdpClient.ConnectingText; }
            set { RdpClient.ConnectingText = value; }
        }

        public override string DisconnectedText
        {
            get { return RdpClient.DisconnectedText; }
            set { RdpClient.DisconnectedText = value; }
        }

        public override int DesktopWidth
        {
            get { return RdpClient.DesktopWidth; }
            set { RdpClient.DesktopWidth = value; }
        }

        public override int DesktopHeight
        {
            get { return RdpClient.DesktopHeight; }
            set { RdpClient.DesktopHeight = value; }
        }

        public override void UpdateSessionDisplaySettings(int width, int height)
        {
            RdpClient.UpdateSessionDisplaySettings(
                (uint)width, (uint)height,
                (uint)width, (uint)height,
                0, 1, 1);
        }

        public override bool FullScreen
        {
            get { return RdpClient.FullScreen; }
            set { RdpClient.FullScreen = value; }
        }
    }
}
