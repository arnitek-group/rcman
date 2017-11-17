using Microsoft.VisualBasic.Devices;
using RemoteConnectionManager.Core.Connections;
using System;
using System.Windows.Forms;

namespace RemoteConnectionManager.Rdp.Clients
{
    internal abstract class RdpClient
    {
        private static int _rdpVersion;

        static RdpClient()
        {
            var os = new ComputerInfo().OSFullName;
            if (os.Contains("Windows 10") || os.Contains("Windows Server 2016"))
            {
                _rdpVersion = 10;
            }
            else if (os.Contains("Windows 8.1") || os.Contains("Windows Server 2012 R2"))
            {
                _rdpVersion = 9;
            }
            else if (os.Contains("Windows 8") || os.Contains("Windows Server 2012"))
            {
                _rdpVersion = 8;
            }
            else if (os.Contains("Windows 7") || os.Contains("Windows Server 2012"))
            {
                _rdpVersion = 7;
            }
            else
            {
                _rdpVersion = 6;
            }
        }

        public static RdpClient Create()
        {
            switch(_rdpVersion)
            {
                case 6:
                case 7:
                case 8:
                    return new RdpClient7();
                case 9:
                case 10:
                    return new RdpClient9();
                default:
                    return new RdpClient5();
            }
        }
        
        public abstract AxHost Client { get; }

        public abstract bool Connected { get; }
        public abstract void Connect();
        public abstract void Disconnect();
        public event EventHandler<DisconnectReason> OnDisconnected;

        protected void AxMsRdpClient_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            var reason = DisconnectReason.ConnectionTerminated;
            // https://social.technet.microsoft.com/wiki/contents/articles/37870.rds-remote-desktop-client-disconnect-codes-and-reasons.aspx
            // https://msdn.microsoft.com/en-us/library/aa382170%28v=vs.85%29.aspx?f=255&MSPPError=-2147217396
            switch (e.discReason)
            {
                case Reason_ClientDisconnect:
                case Reason_ServerDisconnect:
                    reason = DisconnectReason.ConnectionEnded;
                    break;
                case Reason_DisconnectedByUser:
                    reason = DisconnectReason.KickedOut;
                    break;
                case Reason_ServerNotFound:
                    reason = DisconnectReason.ServerNotFound;
                    break;
                case Reason_TimedOut:
                    reason = DisconnectReason.ConnectionTimedOut;
                    break;
            }

            OnDisconnected?.Invoke(this, reason);
        }

        public abstract string Server { get; set; }
        public abstract int Port { get; set; }
        public abstract string Domain { get; set; }
        public abstract string UserName { get; set; }
        public abstract string Password { set; }

        public abstract string ConnectingText { get; set; }
        public abstract string DisconnectedText { get; set; }

        public abstract int DesktopWidth { get; set; }
        public abstract int DesktopHeight { get; set; }
        public abstract void UpdateSessionDisplaySettings(int width, int height);
        public abstract bool FullScreen { get; set; }

        public void SendCtrlAltDel()
        {
            // Source: https://github.com/bosima/RDManager/blob/master/RDManager/MainForm.cs
            Client.Focus();
            new MsRdpClientNonScriptableWrapper(Client.GetOcx()).SendKeys(
                new int[] { 0x1d, 0x38, 0x53, 0x53, 0x38, 0x1d },
                new bool[] { false, false, false, true, true, true, }
            );
        }

        private const int Reason_ClientDisconnect = 0x1;
        private const int Reason_DisconnectedByUser = 0x2;
        private const int Reason_ServerDisconnect = 0x3;
        private const int Reason_ServerNotFound = 0x104;
        private const int Reason_TimedOut = 0x108;
    }

    internal abstract class GenericRdpClient<T>: RdpClient where T: new()
    {
        protected GenericRdpClient()
        {
            RdpClient = new T();
        }

        protected T RdpClient { get; }
    }
}
