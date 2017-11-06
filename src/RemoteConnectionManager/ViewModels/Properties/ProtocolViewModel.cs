using RemoteConnectionManager.Core.Connections;

using System;

namespace RemoteConnectionManager.ViewModels.Properties
{
    public class ProtocolViewModel
    {
        public ProtocolViewModel(Protocol protocol)
        {
            Protocol = protocol;
            DisplayName = Enum.GetName(typeof(Protocol), protocol).ToUpper();
        }

        public Protocol Protocol { get; }
        public string DisplayName { get; set; }
    }
}
