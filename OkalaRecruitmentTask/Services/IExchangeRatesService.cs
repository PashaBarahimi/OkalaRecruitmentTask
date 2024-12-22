using OkalaRecruitmentTask.Models;

namespace OkalaRecruitmentTask.Services
{
    public interface IExchangeRatesService
    {
        Task<ExchangeRates?> GetExchangeRatesAsync();
    }
}
