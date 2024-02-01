using NagadDOTNETIntegration.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NagadDOTNETIntegration
{
    internal class AddNumberDuringCheckout
    {
        private static HttpListener httpListener;
        public static async Task AddNumberDuringCheckoutForTokenization()
        {
            Console.WriteLine("\n");
            Console.Write("Enter cust_id: ");
            string cust_id = Console.ReadLine();

            Console.Write("Enter order_id: ");
            string order_id = Console.ReadLine();

            Console.Write("Enter amount: ");
            string amount = Console.ReadLine();

            Console.WriteLine("Merchant ID:" + GlobalVariables.MerchantId);
            Console.WriteLine("Request Date Time:" + GlobalVariables.RequestDateTime);
            Console.WriteLine("Random Number:" + GlobalVariables.RandomNumber);

            #region Initialize API Data Preparation
            var sensitiveDataJSON = new
            {
                challenge = GlobalVariables.RandomNumber,
                datetime = GlobalVariables.RequestDateTime,
                merchantId = GlobalVariables.MerchantId,
                orderId = order_id
            };
            string sensitiveDataEn = Helper.EncryptWithPublic(JsonConvert.SerializeObject(sensitiveDataJSON));
            string signatureValue = Helper.SignWithMarchentPrivateKey(JsonConvert.SerializeObject(sensitiveDataJSON));
            var postDataJSON = new
            {
                datetime = GlobalVariables.RequestDateTime,
                sensitiveData = sensitiveDataEn,
                signature = signatureValue
            };
            // Serialize JSON data to pass through Initialize API
            string initializeJsonData = JsonConvert.SerializeObject(postDataJSON);
            Console.WriteLine("JSON Data:" + initializeJsonData + "\n");
            #endregion

            #region Call Initialize API
            var responseContent = "";
            string SecureHandShakeAPI = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/initialize/{GlobalVariables.MerchantId}/{order_id}?purpose=ECOM_TXN";
            try
            {
                var httpContent = new StringContent(initializeJsonData, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-4.0.1");
                    httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.0.1");
                    httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(SecureHandShakeAPI, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync();

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("Initialize API Response:" + responseContent + "\n");
            #endregion

            #region Process Initialize API Returned Values

            dynamic response = JObject.Parse(responseContent);
            string returnedSensitiveData = response.sensitiveData;

            string returnedSignature = response.signature;
            byte[] returnedSignatureByte = Encoding.UTF8.GetBytes(returnedSignature);

            //Decrypt Sensitive Data
            string decryptedSensitiveData = Helper.Decrypt(returnedSensitiveData);
            Console.WriteLine("Decrypted Sensitive Data:" + decryptedSensitiveData + "\n");

            #endregion


            JObject json = JObject.Parse(decryptedSensitiveData);
            string paymentReferenceId = (string)json["paymentReferenceId"];
            string challenge = (string)json["challenge"];

            string merchantAdditionalInfo = $"{{\"cust_id\":\"{cust_id}\", \"Service Name\": \"Sheba.xyz\", \"serviceLogoURL\": \"https://my-brand.be/wp-content/uploads/2021/08/my-brand-logo.jpg\"}}";
            /*var merchantAdditionalInfo = new
            {
                cust_id = cust_id,
                ServiceName = "Sheba.xyz",
                ServiceLogoURL = "https://my-brand.be/wp-content/uploads/2021/08/my-brand-logo.jpg"
            };

            string merchantAdditionalInfoJSON = JsonConvert.SerializeObject(merchantAdditionalInfo);*/

            #region Checkout API Data Preparation
            var sensitiveDataJSONCheckout = new
            {
                merchantId = GlobalVariables.MerchantId,
                orderId = order_id,
                customerId = cust_id,
                currencyCode = "050",
                amount = amount,
                challenge = challenge
            };
            Console.WriteLine("Plain Sensitive Data: " + sensitiveDataJSONCheckout);
            string sensitiveDataCheckoutEn = Helper.EncryptWithPublic(JsonConvert.SerializeObject(sensitiveDataJSONCheckout));
            string signatureValueCheckout = Helper.SignWithMarchentPrivateKey(JsonConvert.SerializeObject(sensitiveDataJSONCheckout));
            var postDataCheckoutJSON = new
            {
                sensitiveData = sensitiveDataCheckoutEn,
                signature = signatureValueCheckout,
                merchantCallbackURL = "http://localhost/Nagad_Tokenization/callback.php"
                // additionalMerchantInfo = merchantAdditionalInfo
            };
            // Serialize JSON data to pass through Initialize API
            string checkoutJsonData = JsonConvert.SerializeObject(postDataCheckoutJSON);
            Console.WriteLine("JSON Data:" + checkoutJsonData + "\n");
            #endregion


            #region Call Checkout API
            responseContent = "";
            string CheckoutAPI = $"http://sandbox.mynagad.com:10080/remote-payment-gateway-1.0/api/dfs/check-out/complete/{paymentReferenceId}";
            try
            {
                var httpContent = new StringContent(checkoutJsonData, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("X-KM-Api-Version", "v-4.0.1");
                    httpClient.DefaultRequestHeaders.Add("X-KM-IP-V4", "192.168.0.1");
                    httpClient.DefaultRequestHeaders.Add("X-KM-Client-Type", "PC_WEB");
                    // Do the actual request and await the response
                    var httpResponse = await httpClient.PostAsync(CheckoutAPI, httpContent);

                    // If the response contains content we want to read it!
                    if (httpResponse.Content != null)
                    {
                        responseContent = await httpResponse.Content.ReadAsStringAsync();

                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.WriteLine("Chekcout API Response:" + responseContent + "\n");
            #endregion




        }
    }
}
