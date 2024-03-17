using Server.Models.Momo;

namespace Server.Services
{
    public interface IMomoService
    {
        Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(string fullName, double amount);
        MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
    }
}
