
namespace RemoteConnectionManager.Core
{
    public class ConnectionSettings
    {
        public string DisplayName { get; set; }
        public Protocol Protocol { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public Credentials Credentials { get; set; }
    }
}
