using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace PM_Dashboard
{
   public class EncryptAndDecrypt
    {
        private static byte[] cipherBytes;
        private static byte[] plainByte;
        private static byte[] plainByte2;
        private static byte[] plainKey;
        SymmetricAlgorithm desObj = Rijndael.Create();
        int PasswordLength;

        /// <summary>
        /// Encryption Method to encrypt the password
        /// </summary>
        /// <param name="Password">User Password</param>
        /// <returns>Encrypted Password to display</returns>
        public string Encrypt(string Password)
        {
            //First we Convert plain text to byte array
            plainByte = Encoding.Unicode.GetBytes(Password);

            plainKey = Encoding.Unicode.GetBytes("0123456789abcdef");

            PasswordLength = Password.Length;
            desObj.Key = plainKey;

            desObj.Mode = CipherMode.CBC;

            //Memory Stream to hold the bytes
            MemoryStream ms = new MemoryStream();

            //creating our decoder to write to the stream
            CryptoStream cs = new CryptoStream(ms, desObj.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(plainByte, 0, plainByte.Length);
            cs.Close();
            cipherBytes = ms.ToArray();

            //Convert our stream to string value
            return Encoding.Unicode.GetString(cipherBytes);
        }

        /// <summary>
        /// Decryption Method to decrypt the encrypted password
        /// </summary>
        /// <returns>the decrypted Password</returns>
        public string Decrypt()
        {
            MemoryStream memoryStream = new MemoryStream(cipherBytes);
            CryptoStream decrypt = new CryptoStream(memoryStream, desObj.CreateDecryptor(), CryptoStreamMode.Read);
            decrypt.Read(cipherBytes, 0, cipherBytes.Length);
            decrypt.Close();
            plainByte2 = memoryStream.ToArray();
            string decryptedText = Encoding.Unicode.GetString(plainByte2);
            return decryptedText.Substring(0, this.PasswordLength);
        }
    }
}
