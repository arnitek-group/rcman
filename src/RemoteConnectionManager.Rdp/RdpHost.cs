using RemoteConnectionManager.Rdp.Clients;
using System.Windows.Forms;

namespace RemoteConnectionManager.Rdp
{
    public partial class RdpHost : UserControl
    {
        public RdpHost()
        {
            Rdp = RdpClient.Create();

            InitializeComponent();
        }

        internal RdpClient Rdp { get; }
    }
}
