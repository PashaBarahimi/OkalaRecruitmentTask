using OkalaRecruitmentTask.Models;

namespace OkalaRecruitmentTask.Services;

public interface ICryptoPriceService
{
    Task<CryptoPrice> GetPriceAsync(string code);
}