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

            PaymentLinkItem item = new PaymentLinkItem {
                Name = transaction.Plan?.Name ?? "FirstAidPus Subscription",
                Quantity = 1,
                Price = (int)transaction.Amount
            };
            List<PaymentLinkItem> items = new List<PaymentLinkItem> { item };

            CreatePaymentLinkRequest paymentData = new CreatePaymentLinkRequest {
                OrderCode = transaction.Id,
                Amount = (int)transaction.Amount,
                Description = $"Thanh toan don {transaction.Id}",
                Items = items,
                CancelUrl = cancelUrl,
                ReturnUrl = returnUrl
            };

            CreatePaymentLinkResponse createPayment = await _payOS.PaymentRequests.CreateAsync(paymentData);
            return createPayment;
        }

        public async Task<WebhookData> VerifyPaymentWebhookData(Webhook webhookBody)
        {
            return await _payOS.Webhooks.VerifyAsync(webhookBody);
        }
    }
}
