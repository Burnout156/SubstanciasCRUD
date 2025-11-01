using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SubstanciasDatabase.Criptografia
{
    // Criptografa/Descriptografa strings com AES-GCM (nonce + tag concatenados)
    public sealed class AesGcmStringProtector
    {
        private readonly byte[] _key; // 32 bytes para AES-256

        public AesGcmStringProtector(string base64Key)
        {
            _key = Convert.FromBase64String(base64Key);
            if (_key.Length != 32)
                throw new ArgumentException("A chave AES deve ter 32 bytes (Base64 de 256 bits).");
        }

        public string? Encrypt(string? plain)
        {
            if (string.IsNullOrEmpty(plain)) return plain;

            byte[] plaintext = Encoding.UTF8.GetBytes(plain);
            byte[] nonce = RandomNumberGenerator.GetBytes(12);
            byte[] ciphertext = new byte[plaintext.Length];
            byte[] tag = new byte[16];

            using var aes = new AesGcm(_key);
            aes.Encrypt(nonce, plaintext, ciphertext, tag);

            // Formato: base64(nonce|tag|ciphertext)
            byte[] packed = new byte[nonce.Length + tag.Length + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, packed, 0, nonce.Length);
            Buffer.BlockCopy(tag, 0, packed, nonce.Length, tag.Length);
            Buffer.BlockCopy(ciphertext, 0, packed, nonce.Length + tag.Length, ciphertext.Length);

            return Convert.ToBase64String(packed);
        }

        public string? Decrypt(string? cipher)
        {
            if (string.IsNullOrEmpty(cipher)) return cipher;

            byte[] packed = Convert.FromBase64String(cipher);
            byte[] nonce = new byte[12];
            byte[] tag = new byte[16];
            byte[] ciphertext = new byte[packed.Length - nonce.Length - tag.Length];

            Buffer.BlockCopy(packed, 0, nonce, 0, nonce.Length);
            Buffer.BlockCopy(packed, nonce.Length, tag, 0, tag.Length);
            Buffer.BlockCopy(packed, nonce.Length + tag.Length, ciphertext, 0, ciphertext.Length);

            byte[] plaintext = new byte[ciphertext.Length];
            using var aes = new AesGcm(_key);
            aes.Decrypt(nonce, ciphertext, tag, plaintext);

            return Encoding.UTF8.GetString(plaintext);
        }
    }
}
