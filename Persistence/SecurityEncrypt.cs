using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace BAU_PROTO.Persistence
{
    // This class provides methods for encrypting and decrypting strings using AES 128 - CBC - PKCS7 encryption.
    public class SecurityEncrypt
    {
        public static string Encrypt(string plainText, string key)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException("Target string cannot be null or empty.", nameof(plainText));

            byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            int nonceSize = AesGcm.NonceByteSizes.MaxSize;
            int tagSize = AesGcm.TagByteSizes.MaxSize;
            int cipherSize = plainBytes.Length;

            if (keyBytes.Length != 32)
            {
                throw new ArgumentException("key length should be 16");
            }

            int encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
            Span<byte> encryptedData = encryptedDataLength < 1024
                ? stackalloc byte[encryptedDataLength]
                : new byte[encryptedDataLength].AsSpan();

            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            RandomNumberGenerator.Fill(nonce);

            using (AesGcm aes = new AesGcm(keyBytes, tagSize))
            {
                aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);
                return Convert.ToBase64String(encryptedData);
            }
        }

        public static string Decrypt(string cipherText, string key)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentException("Target string cannot be null or empty.", nameof(cipherText));

            byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes(key));

            Span<byte> encryptedData = Convert.FromBase64String(cipherText).AsSpan();

            if (keyBytes.Length != 32)
            {
                throw new ArgumentException("key length should be 16");
            }

            int nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(0, 4));
            int tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
            int cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            Span<byte> plainBytes = cipherSize < 1024
                ? stackalloc byte[cipherSize]
                : new byte[cipherSize];
            using (AesGcm aes = new AesGcm(keyBytes, tagSize))
            {
                aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

                return Encoding.UTF8.GetString(plainBytes);
            }
        }
    }
}
