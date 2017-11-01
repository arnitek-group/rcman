
namespace RemoteConnectionManager.Core
{
    public class ConnectionSettings
    {
        public Protocol Protocol { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public Credentials Credentials { get; set; }
    }
}
