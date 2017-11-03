using System;
using System.IO;
using System.Security.Cryptography;

namespace RemoteConnectionManager.Core
{
    public static class Security
    {
        private static readonly byte[] Key = { 230, 60, 173, 137, 52, 240, 64, 112, 5, 167, 28, 240, 22, 233, 94, 8, 163, 155, 1, 79, 58, 227, 6, 78, 155, 182, 94, 97, 169, 102, 77, 28 };
        private static readonly byte[] IV = { 205, 80, 156, 11, 96, 56, 26, 98, 83, 61, 119, 210, 162, 150, 84, 19 };

        public static string EncryptText(string text)
        {
            byte[] encrypted;
            using (var crypto = Rijndael.Create())
            {
                crypto.Key = Key;
                crypto.IV = IV;

                var encryptor = crypto.CreateEncryptor(crypto.Key, crypto.IV);
                using (var mStream = new MemoryStream())
                {
                    using (var cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(cStream))
                        {
                            swEncrypt.Write(text);
                        }
                        encrypted = mStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptText(string encryptedText)
        {
            string decrypted;
            using (var crypto = Rijndael.Create())
            {
                crypto.Key = Key;
                crypto.IV = IV;

                var encryptor = crypto.CreateDecryptor(crypto.Key, crypto.IV);
                using (var mStream = new MemoryStream(Convert.FromBase64String(encryptedText)))
                using (var cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(cStream))
                {
                    decrypted = srDecrypt.ReadToEnd();
                }
            }
            return decrypted;
        }
    }
}
