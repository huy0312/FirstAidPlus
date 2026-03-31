using FirstAidPlus.Models;

namespace FirstAidPlus.Services
{
    public interface IMoMoService
    {
        Task<string> CreatePaymentUrl(HttpContext context, Transaction transaction);
        (bool success, string orderId, string transId) ValidateCallback(IQueryCollection collection);
    }
}
