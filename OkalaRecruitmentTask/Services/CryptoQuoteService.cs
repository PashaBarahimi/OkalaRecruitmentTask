using OkalaRecruitmentTask.Models;

namespace OkalaRecruitmentTask.Services;

public class CryptoQuoteService(ICryptoPriceService cryptoPriceService, ICurrencyRatesService currencyRatesService, ILogger<CryptoQuoteService> logger) : ICryptoQuoteService
{
    public async Task<CryptoQuote> GetQuoteAsync(string code)
    {
        logger.LogInformation("Getting quote for {Code}", code);

        var price = await cryptoPriceService.GetPriceAsync(code);
        var currencyRates = await currencyRatesService.GetCurrencyRatesAsync();

        Dictionary<string, decimal> quote = [];
        foreach (var (currency, rate) in currencyRates.Rates)
        {
            quote[currency] = price.PriceBase * rate;
        }

        return new CryptoQuote
        {
            Code = code,
            Quote = quote
        };
    }
}
