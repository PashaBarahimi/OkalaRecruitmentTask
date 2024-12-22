using OkalaRecruitmentTask.Models;
using Newtonsoft.Json.Linq;

namespace OkalaRecruitmentTask.Services
{
    public class CryptoPriceService(ILogger<CryptoPriceService> logger, IConfiguration configuration) : ICryptoPriceService
    {
        private readonly ILogger<CryptoPriceService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;

        public async Task<CryptoPrice?> GetPriceAsync(string code)
        {
            var baseCurrency = _configuration["Quotes:Currencies:Base"];
            if (string.IsNullOrEmpty(baseCurrency))
            {
                _logger.LogWarning("Base currency not found in configuration");
                return null;
            }
            _logger.LogInformation("Getting price for {Code} in {BaseCurrency}", code, baseCurrency);

            var price = await GetPriceInCurrencyAsync(code, baseCurrency);
            if (price == null)
            {
                _logger.LogWarning("Price for {Code} not found", code);
                return null;
            }
            _logger.LogInformation("Price for {Code} found", code);

            return price;
        }

        public async Task<CryptoPrice?> GetPriceInCurrencyAsync(string code, string currency)
        {
            var url = _configuration["Quotes:APIs:CoinMarketCap:URL"];
            if (string.IsNullOrEmpty(url))
            {
                _logger.LogCritical("CoinMarketCap API URL not found in configuration");
                return null;
            }

            var apiKey = _configuration["Quotes:APIs:CoinMarketCap:APIKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogCritical("CoinMarketCap API key not found in configuration (IMPORTANT: Use Dotnet user-secrets)");
                return null;
            }

            _logger.LogInformation("Getting price for {Code} in {Currency} from {URL}", code, currency, url);

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);

            var response = await client.GetAsync($"{url}?symbol={code}&convert={currency}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get price for {Code} in {Currency} from {URL}", code, currency, url);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var price = json["data"]?[code]?[0]?["quote"]?[currency]?["price"]?.Value<decimal>();
            if (price == null)
            {
                _logger.LogWarning("Price for {Code} in {Currency} not found", code, currency);
                return null;
            }

            _logger.LogInformation("Price for {Code} in {Currency} found", code, currency);

            return new CryptoPrice
            {
                Code = code,
                BaseCurrency = currency,
                PriceBase = price.Value
            };
        }
    }
}
