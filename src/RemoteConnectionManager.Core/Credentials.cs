
namespace RemoteConnectionManager.Core
{
    public class Credentials
    {
        public string Domain { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }

        public void SetPassword(string plainText)
        {
            Password = Security.EncryptText(plainText);
        }

        public string GetPassword()
        {
            return Security.DecryptText(Password);
        }
    }
}
