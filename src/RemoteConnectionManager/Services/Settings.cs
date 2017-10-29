using RemoteConnectionManager.Core;

namespace RemoteConnectionManager.Services
{
    public class Settings
    {
        public Credentials[] Credentials { get; set; }
        public ConnectionSettings[] ConnectionSettings { get; set; }
    }
}