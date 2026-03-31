using FirstAidPlus.Helpers;
using FirstAidPlus.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace FirstAidPlus.Services
{
    public class MoMoService : IMoMoService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public MoMoService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string> CreatePaymentUrl(HttpContext context, Transaction transaction)
        {
            string endpoint = _configuration["Momo:ApiEndpoint"];
            string partnerCode = _configuration["Momo:PartnerCode"];
            string accessKey = _configuration["Momo:AccessKey"];
            string secretKey = _configuration["Momo:SecretKey"];
            string orderInfo = transaction.OrderDescription ?? "Thanh toan don hang";
            string redirectUrl = _configuration["Momo:ReturnUrl"];
            if (redirectUrl != null && redirectUrl.Contains("localhost"))
            {
                redirectUrl = $"{context.Request.Scheme}://{context.Request.Host}/Subscription/MomoCallback";
            }
            string ipnUrl = _configuration["Momo:NotifyUrl"];
            if (ipnUrl != null && ipnUrl.Contains("localhost"))
            {
                ipnUrl = $"{context.Request.Scheme}://{context.Request.Host}/Subscription/MomoNotify";
            }
            string requestType = "captureWallet";
            string amount = ((long)transaction.Amount).ToString();
            string orderId = transaction.MomoOrderId ?? Guid.NewGuid().ToString(); // Use existing or generate new
            transaction.MomoOrderId = orderId; // Ensure it's saved back if needed, though usually saved before calling this
            
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            // Before sign HMAC SHA256 signature
            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&ipnUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType;

            string signature = MoMoLibrary.HmacSHA256(rawHash, secretKey);

            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "FirstAidPlus" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "vi" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }
            };

            var requestContent = new StringContent(message.ToString(), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, requestContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var jmessage = JObject.Parse(jsonResponse);

            if (jmessage["payUrl"] != null)
            {
                return jmessage["payUrl"].ToString();
            }
            
            // Return error message if payUrl is missing
            return "Error: " + (jmessage["message"]?.ToString() ?? jmessage["localMessage"]?.ToString() ?? jsonResponse);
        }

        public (bool success, string orderId, string transId) ValidateCallback(IQueryCollection collection)
        {
            string accessKey = _configuration["Momo:AccessKey"];
            string secretKey = _configuration["Momo:SecretKey"];

            string partnerCode = collection["partnerCode"];
            string orderId = collection["orderId"];
            string requestId = collection["requestId"];
            string amount = collection["amount"];
            string orderInfo = collection["orderInfo"];
            string orderType = collection["orderType"];
            string transId = collection["transId"];
            string resultCode = collection["resultCode"];
            string message = collection["message"];
            string payType = collection["payType"];
            string responseTime = collection["responseTime"];
            string extraData = collection["extraData"];
            string signature = collection["signature"];

            string rawHash = "accessKey=" + accessKey +
                             "&amount=" + amount +
                             "&extraData=" + extraData +
                             "&message=" + message +
                             "&orderId=" + orderId +
                             "&orderInfo=" + orderInfo +
                             "&orderType=" + orderType +
                             "&partnerCode=" + partnerCode +
                             "&payType=" + payType +
                             "&requestId=" + requestId +
                             "&responseTime=" + responseTime +
                             "&resultCode=" + resultCode +
                             "&transId=" + transId;

            string checkSignature = MoMoLibrary.HmacSHA256(rawHash, secretKey);

            // Verify signature and resultCode
            if (checkSignature.Equals(signature) && resultCode == "0")
            {
                return (true, orderId, transId);
            }

            return (false, orderId, transId);
        }
    }
}
