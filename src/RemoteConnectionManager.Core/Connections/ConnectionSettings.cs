
namespace RemoteConnectionManager.Core.Connections
{
    public class ConnectionSettings: Credentials
    {
        public Protocol Protocol { get; set; }
        public string Server { get; set; }
        public string Port { get; set; }
        public Credentials Credentials { get; set; }

        /// <summary>
        /// Determines what credentials to use.
        /// </summary>
        public Credentials GetCredentials()
        {
            if (!string.IsNullOrEmpty(Domain) ||
                !string.IsNullOrEmpty(Username) ||
                !string.IsNullOrEmpty(Password) ||
                !string.IsNullOrEmpty(KeyFile))
            {
                return new Credentials
                {
                    Domain = Domain,
                    Username = Username,
                    Password = Password,
                    KeyFile = KeyFile
                };
            }

            return Credentials;
        }
    }
}
