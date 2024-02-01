using NagadDOTNETIntegration.Models;
using System;

namespace NagadDOTNETIntegration
{
    internal class DecryptToken
    {
        public static void DecryptTokenByKey()
        {
            Console.WriteLine("\n");
            Console.Write("Enter Token: ");
            string encryptedToken = Console.ReadLine();

            byte[] keyBytes = StringToByteArray(GlobalVariables.key);
            byte[] ivBytes = StringToByteArray(GlobalVariables.iv);
            byte[] encryptedTokenBytes = StringToByteArray(encryptedToken);

            string decryptedToken = Helper.DecryptWithAES(encryptedTokenBytes, keyBytes, ivBytes);
            Console.WriteLine("\nDecrypted Token: "+decryptedToken);
            
        }

        private static byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }


    
}
}
