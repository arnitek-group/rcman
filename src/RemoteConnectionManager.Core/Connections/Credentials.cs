
namespace RemoteConnectionManager.Core.Connections
{
    public class Credentials
    {
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public void SetPassword(string plainText)
        {
            Password = string.IsNullOrEmpty(plainText) 
                ? null 
                : Security.EncryptText(plainText);
        }

        public string GetPassword()
        {
            return Security.DecryptText(Password);
        }
    }
}
