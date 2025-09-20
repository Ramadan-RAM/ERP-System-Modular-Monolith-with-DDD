using System.Security.Cryptography;
using System.Text;

namespace Users.Infrastructure.Common.Security
{

    public static class EncryptionHelper
    {
        private static readonly string Key = "0mkjudxRE!G22;r5"; // 🛡️ Use at least 16 characters

        public static string Encrypt(string plainText)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
            using var aes = Aes.Create();
            aes.Key = keyBytes.Take(16).ToArray(); // 128-bit key
            aes.GenerateIV();
            using var encryptor = aes.CreateEncryptor();

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            return Convert.ToBase64String(aes.IV.Concat(encryptedBytes).ToArray());
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] fullCipher = Convert.FromBase64String(encryptedText);
            byte[] iv = fullCipher.Take(16).ToArray();
            byte[] cipher = fullCipher.Skip(16).ToArray();

            byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
            using var aes = Aes.Create();
            aes.Key = keyBytes.Take(16).ToArray();
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();

            byte[] decryptedBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
