using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Helper
{
    public class CryptorEngineHelper
    {
        private static string cipherCode = "HR$2pIjHR$2pIj12";
        private static string initVector = "HR$2pIjHR$2pIj12";

        public static string Encrypt(string plainText)
        {
            byte[] keyBytes = Encoding.ASCII.GetBytes(cipherCode);
            return Encrypt(plainText, keyBytes, initVector);
        }

        public static string Encrypt(string plainText, byte[] keyBytes, string initVector)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (Aes symmetricKey = Aes.Create())
            {
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] cipherTextBytes = memoryStream.ToArray();
                        string cipherText = Convert.ToBase64String(cipherTextBytes);
                        return cipherText;
                    }
                }
            }
        }

        public static string Decrypt(string cipherText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cipherText))
                    return string.Empty;

                byte[] keyBytes = Encoding.ASCII.GetBytes(cipherCode);
                return Decrypt(cipherText, keyBytes, initVector);
            }
            catch (Exception ex)
            {
                // Log error but don't throw - return empty string
                try 
                { 
                    Console.WriteLine($"CryptorEngineHelper.Decrypt failed: {ex.Message}");
                    Console.WriteLine($"Input: {cipherText}");
                } 
                catch { }
                return string.Empty;
            }
        }

        public static string Decrypt(string cipherText, byte[] keyBytes, string initVector)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            using (Aes symmetricKey = Aes.Create())
            {
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                        string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        return plainText;
                    }
                }
            }
        }
    }
}
