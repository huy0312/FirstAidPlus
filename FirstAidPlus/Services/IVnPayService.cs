using FirstAidPlus.Models;

namespace FirstAidPlus.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContext context, Transaction transaction);
        bool ValidateCallback(IQueryCollection collections);
    }
}
