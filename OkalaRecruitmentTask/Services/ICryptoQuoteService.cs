using OkalaRecruitmentTask.Models;

namespace OkalaRecruitmentTask.Services;

public interface ICryptoQuoteService
{
    Task<CryptoQuote> GetQuoteAsync(string code);
}
