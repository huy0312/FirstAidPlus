using FirstAidPlus.Models;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FirstAidPlus.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOSClient _payOS;

        public PayOSService(PayOSClient payOS)
        {
            _payOS = payOS;
        }

        public async Task<CreatePaymentLinkResponse> CreatePaymentLink(HttpContext context, Transaction transaction)
        {
            var domain = $"{context.Request.Scheme}://{context.Request.Host}";
            var returnUrl = $"{domain}/Subscription/PayOSCallback";
            var cancelUrl = $"{domain}/Subscription/Index";

            // Safety check for amount
            int amount = (int)transaction.Amount;
            if (amount < 2000) amount = 2000;

            PaymentLinkItem item = new PaymentLinkItem {
                Name = (transaction.Plan?.Name ?? "FirstAidSub").Substring(0, Math.Min(20, (transaction.Plan?.Name ?? "FirstAidSub").Length)),
                Quantity = 1,
                Price = amount
            };
            List<PaymentLinkItem> items = new List<PaymentLinkItem> { item };

            // PayOS OrderCode MUST be a number and MUST be unique. 
            // We use Timestamp + ID to ensure uniqueness across tests.
            string uniquePrefix = DateTime.Now.ToString("HHmmss");
            long uniqueOrderCode = long.Parse(uniquePrefix + transaction.Id);

            CreatePaymentLinkRequest paymentData = new CreatePaymentLinkRequest {
                OrderCode = uniqueOrderCode,
                Amount = amount,
                Description = $"HD{transaction.Id}",
                Items = items,
                CancelUrl = cancelUrl,
                ReturnUrl = returnUrl
            };

            try 
            {
                CreatePaymentLinkResponse createPayment = await _payOS.PaymentRequests.CreateAsync(paymentData);
                return createPayment;
            }
            catch (Exception ex)
            {
                // Capture detailed error for debugging
                throw new Exception($"PayOS Create Error: {ex.Message}. Request Data: {Newtonsoft.Json.JsonConvert.SerializeObject(paymentData)}");
            }
        }

        public async Task<WebhookData> VerifyPaymentWebhookData(Webhook webhookBody)
        {
            return await _payOS.Webhooks.VerifyAsync(webhookBody);
        }
    }
}
