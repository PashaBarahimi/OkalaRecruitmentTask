using OkalaRecruitmentTask.Models;

namespace OkalaRecruitmentTask.Services
{
    public class ExchangeRatesService(ILogger<ExchangeRatesService> logger) : IExchangeRatesService
    {
        private readonly ILogger<ExchangeRatesService> _logger = logger;

        public Task<ExchangeRates?> GetExchangeRateAsync()
        {
            _logger.LogInformation("Getting exchange rates");

            // This is a placeholder for a real API call
            var exchangeRates = new ExchangeRates
            {
                BaseCurrency = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    ["EUR"] = 0.85m,
                    ["GBP"] = 0.75m,
                    ["JPY"] = 110.0m
                }
            };

            _logger.LogInformation("Exchange rates found");

            return Task.FromResult<ExchangeRates?>(exchangeRates);
        }
    }
}
