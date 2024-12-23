using OkalaRecruitmentTask.Models;

namespace OkalaRecruitmentTask.Services;

public interface ICurrencyRatesService
{
    Task<CurrencyRates> GetCurrencyRatesAsync();
}