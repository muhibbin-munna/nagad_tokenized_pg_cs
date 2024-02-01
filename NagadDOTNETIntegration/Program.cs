using NagadDOTNETIntegration.Functions;
using NagadDOTNETIntegration.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace NagadDOTNETIntegration
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.WriteLine("Nagad Tokenized Payment Gateway!\n");
            Console.Write("1. Add Number For Tokenization\n");
            Console.Write("2. Add Number During Regular Checkout\n");
            Console.Write("3. Decrypt Token\n\n");
            Console.WriteLine("Enter your choice: ");
            string option = Console.ReadLine();
            
            if (option.Equals("1"))
            {
                await AddNumber.AddNumberForTokenization();
            }
            if (option.Equals("2"))
            {
                await AddNumberDuringCheckout.AddNumberDuringCheckoutForTokenization();
            }
            if (option.Equals("3"))
            {
                DecryptToken.DecryptTokenByKey();
            }
        }

       
    }
}
