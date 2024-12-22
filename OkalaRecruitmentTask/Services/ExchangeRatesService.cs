using OkalaRecruitmentTask.Models;
using Newtonsoft.Json.Linq;

namespace OkalaRecruitmentTask.Services
{
    public class ExchangeRatesService(ILogger<ExchangeRatesService> logger, IConfiguration configuration) : IExchangeRatesService
    {
        private readonly ILogger<ExchangeRatesService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public Task<ExchangeRates?> GetExchangeRatesAsync()
        {
            _logger.LogInformation("Getting exchange rates");

            var baseCurrency = _configuration["Quotes:Currencies:Base"];
            if (string.IsNullOrEmpty(baseCurrency))
            {
                _logger.LogWarning("Base currency not found in configuration");
                return Task.FromResult<ExchangeRates?>(null);
            }

            var exchangeRates = GetExchangeRatesWithBaseAsync(baseCurrency);
            _logger.LogInformation("Exchange rates found");

            return exchangeRates;
        }

        public async Task<ExchangeRates?> GetExchangeRatesWithBaseAsync(string baseCurrency)
        {
            _logger.LogInformation("Getting exchange rates with base {BaseCurrency}", baseCurrency);

            var url = _configuration["Quotes:APIs:ExchangeRates:URL"];
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogCritical("Exchange rates API URL not found in configuration");
                return null;
            }

            var apiKey = _configuration["Quotes:APIs:ExchangeRates:APIKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogCritical("Exchange rates API key not found in configuration (IMPORTANT: Use Dotnet user-secrets)");
                return null;
            }

            var requiredSymbols = _configuration.GetSection("Quotes:Currencies:Required").Get<string[]>();
            if (requiredSymbols is null || requiredSymbols.Length == 0)
            {
                _logger.LogWarning("Required currencies not found in configuration");
                return null;
            }
            var symbols = string.Join(',', requiredSymbols);

            _logger.LogInformation("Getting exchange rates with base {BaseCurrency} from {URL}", baseCurrency, url);

            using var client = new HttpClient();
            var response = await client.GetAsync($"{url}?base={baseCurrency}&symbols={symbols}&access_key={apiKey}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get exchange rates with base {BaseCurrency} from {URL}", baseCurrency, url);
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var rates = json["rates"]?.ToObject<Dictionary<string, decimal>>();
            if (rates is null)
            {
                _logger.LogWarning("Exchange rates with base {BaseCurrency} not found", baseCurrency);
                return null;
            }
            _logger.LogInformation("Exchange rates with base {BaseCurrency} found", baseCurrency);

            return new ExchangeRates
            {
                BaseCurrency = baseCurrency,
                Rates = rates
            };
        }
    }
}
