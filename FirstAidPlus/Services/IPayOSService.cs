using FirstAidPlus.Models;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FirstAidPlus.Services
{
    public interface IPayOSService
    {
        Task<CreatePaymentLinkResponse> CreatePaymentLink(HttpContext context, Transaction transaction);
        Task<WebhookData> VerifyPaymentWebhookData(Webhook webhookBody);
    }
}
