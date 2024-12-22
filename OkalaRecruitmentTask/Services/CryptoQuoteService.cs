using OkalaRecruitmentTask.Models;

namespace OkalaRecruitmentTask.Services;

public class CryptoQuoteService(ICryptoPriceService cryptoPriceService, IExchangeRatesService exchangeRatesService, ILogger<CryptoQuoteService> logger) : ICryptoQuoteService
{
    private readonly ICryptoPriceService _cryptoPriceService = cryptoPriceService;
    private readonly IExchangeRatesService _exchangeRatesService = exchangeRatesService;
    private readonly ILogger<CryptoQuoteService> _logger = logger;

    public async Task<CryptoQuote?> GetQuoteAsync(string code)
    {
        _logger.LogInformation("Getting quote for {Code}", code);

        var price = await _cryptoPriceService.GetPriceAsync(code);
        if (price is null)
        {
            _logger.LogWarning("Price for {Code} not found", code);
            return null;
        }
        _logger.LogInformation("Price for {Code} found", code);

        var exchangeRates = await _exchangeRatesService.GetExchangeRateAsync();
        if (exchangeRates is null)
        {
            _logger.LogWarning("Exchange rates not found");
            return null;
        }
        _logger.LogInformation("Exchange rates found");

        Dictionary<string, decimal> quote = [];
        foreach ((string currency, decimal rate) in exchangeRates.Rates)
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
