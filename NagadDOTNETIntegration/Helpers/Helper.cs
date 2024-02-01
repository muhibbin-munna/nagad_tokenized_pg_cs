using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NagadDOTNETIntegration.Models;

namespace NagadDOTNETIntegration
{
    internal class Helper

    {
        public static string nagadPublicKey = GlobalVariables.nagadPublicKey;
        public static string marchentPrivateKey = GlobalVariables.marchentPrivateKey;

        #region EncryptWithPublicKey
        public static string EncryptWithPublic(string baseText)
        {
            try
            {
                RSA cipher = getKey(0);
                var plaintext = baseText;
                byte[] data = Encoding.UTF8.GetBytes(plaintext);

                byte[] cipherText = cipher.Encrypt(data, RSAEncryptionPadding.Pkcs1);
                var encryptedData = Convert.ToBase64String(cipherText);
                return encryptedData;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion


        #region SignWithMarchentPrivateKey
        public static string SignWithMarchentPrivateKey(string baseText)
        {
            try
            {
                var rsa = getKey(1);
                byte[] dataBytes = Encoding.UTF8.GetBytes(baseText);
                var signatureBytes = rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signatureBytes);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion


        #region RSA Encryption Keys
        private static RSA getKey(int num)
        {
            try
            {
                if (num == 0)
                {
                    var publicKeyBytes = Convert.FromBase64String(nagadPublicKey);
                    int myarray;
                    var rsa = RSA.Create();

                    rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out myarray);
                    return rsa;
                }
                if (num == 1)
                {
                    var privateKeyBytes = Convert.FromBase64String(marchentPrivateKey);
                    int myarray;
                    var rsa = RSA.Create();

                    rsa.ImportPkcs8PrivateKey(privateKeyBytes, out myarray);
                    return rsa;
                }

            }
            catch (CryptographicException e)
            {

                Console.WriteLine(e.Message);
            }

            return null;
        }
        #endregion

        #region Decrypt
        public static string Decrypt(string plainText)
        {
            var rsa = getKey(1);
            if (rsa == null)
            {
                throw new Exception("_privateKeyRsaProvider is null");
            }
            string decryptedData = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(plainText), RSAEncryptionPadding.Pkcs1));
            return decryptedData;
        }


        #endregion

        #region DecryptWithAESandIV


        public static string DecryptWithAES(byte[] encryptedTokenBytes, byte[] keyBytes, byte[] ivBytes)
        {
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = keyBytes;
                aesAlg.IV = ivBytes;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedTokenBytes, 0, encryptedTokenBytes.Length);
                    string decryptedTokenString = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedTokenString;
                }
            }
        }


        #endregion

    }
}
