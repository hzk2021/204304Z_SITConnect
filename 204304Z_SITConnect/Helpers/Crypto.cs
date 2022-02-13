using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace _204304Z_SITConnect.Helpers
{
    public static class Crypto
    {
        public static string GetRandomSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] saltByte = new byte[8];
            rng.GetBytes(saltByte);

            return Convert.ToBase64String(saltByte);
        }

        public static string GetHashedString(string text, string salt)
        {
            SHA512Managed sha512Hash = new SHA512Managed();
            string clearTextWithSalt = text + salt;

            byte[] hashTextandSalt = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(clearTextWithSalt));
            return Convert.ToBase64String(hashTextandSalt);

        }

        public static Tuple<byte[], byte[]> GetRandomIVAndKey()
        {
            byte[] IV;
            byte[] Key;

            // Generate IV & Key
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            IV = rijndaelCipher.IV;
            Key = rijndaelCipher.Key;

            return Tuple.Create(IV, Key);
        }

        public static string GetEncryptedText(string clear_text, byte[] IV, byte[] Key)
        {
            string cipherText;

            try
            {
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                rijndaelCipher.IV = IV;
                rijndaelCipher.Key = Key;

                ICryptoTransform encryptTransform = rijndaelCipher.CreateEncryptor();
                byte[] clearText = Encoding.UTF8.GetBytes(clear_text);
                cipherText = Convert.ToBase64String(encryptTransform.TransformFinalBlock(clearText, 0, clearText.Length));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return cipherText;
        }
    }
}