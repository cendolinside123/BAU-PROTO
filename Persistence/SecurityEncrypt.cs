using System.Security.Cryptography;
using System.Text;

namespace BAU_PROTO.Persistence
{
    // This class provides methods for encrypting and decrypting strings using AES 128 - CBC - PKCS7 encryption.
    public class SecurityEncrypt
    {
        public static string Encrypt(string plainText, string key, string iv)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Target string cannot be null or empty.", nameof(plainText));

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);


            if (ivBytes.Length != 16)
            {
                throw new ArgumentException("IV length should be 16");
            }

            if (keyBytes.Length != 16)
            {
                throw new ArgumentException("key length should be 16");
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string key, string iv)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Target string cannot be null or empty.", nameof(cipherText));

            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            if (ivBytes.Length != 16)
            {
                throw new ArgumentException("IV length should be 16");
            }

            if (keyBytes.Length != 16)
            {
                throw new ArgumentException("key length should be 16");
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new MemoryStream())
                {
                    cs.CopyTo(sr);
                    return Encoding.UTF8.GetString(sr.ToArray());
                }
            }
        }
    }
}
