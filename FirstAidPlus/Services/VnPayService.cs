using FirstAidPlus.Helpers;
using FirstAidPlus.Models;

namespace FirstAidPlus.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(HttpContext context, Transaction transaction)
        {
            var timeZoneId = "SE Asia Standard Time";
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["VnPay:ReturnUrl"];
            if (urlCallBack != null && urlCallBack.Contains("localhost"))
            {
                urlCallBack = $"{context.Request.Scheme}://{context.Request.Host}/Subscription/PaymentCallback";
            }

            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", _configuration["VnPay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((long)transaction.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", transaction.OrderDescription ?? $"Thanh toan don hang {transaction.Id}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", transaction.VnPayTxnRef);

            var paymentUrl = pay.CreateRequestUrl(_configuration["VnPay:Url"], _configuration["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public bool ValidateCallback(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    pay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = collections.FirstOrDefault(p => p.Key == "vnp_ResponseCode").Value;
            
            // Validate Signature
            bool checkSignature = pay.ValidateSignature(vnp_SecureHash, _configuration["VnPay:HashSecret"]);
            
            if (!checkSignature) return false;

            return vnp_ResponseCode == "00";
        }
    }
}
