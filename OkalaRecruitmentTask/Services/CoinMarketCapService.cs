using OkalaRecruitmentTask.Models;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace OkalaRecruitmentTask.Services;

public class CoinMarketCapService(ILogger<CoinMarketCapService> logger, IConfiguration configuration) : ICryptoPriceService
{
    public async Task<CryptoPrice> GetPriceAsync(string code)
    {
        logger.LogInformation("Getting price for {Code}", code);

        var baseCurrency = configuration["Quotes:Currencies:Base"];
        if (string.IsNullOrEmpty(baseCurrency))
        {
            logger.LogCritical("Base currency not found in the configuration");
            throw new ConfigurationErrorsException("Base currency not found in the configuration");
        }

        return await GetPriceInCurrencyAsync(code, baseCurrency);
    }

    private async Task<CryptoPrice> GetPriceInCurrencyAsync(string code, string currency)
    {
        var url = configuration["Quotes:APIs:CoinMarketCap:URL"];
        if (string.IsNullOrEmpty(url))
        {
            logger.LogCritical("CoinMarketCap API URL not found in the configuration");
            throw new ConfigurationErrorsException("CoinMarketCap API URL not found in the configuration");
        }

        var apiKey = configuration["Quotes:APIs:CoinMarketCap:APIKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            logger.LogCritical("CoinMarketCap API key not found in configuration (CAUTION: Use dotnet user-secrets)");
            throw new ConfigurationErrorsException("CoinMarketCap API key not found in the configuration");
        }

        return await FetchPriceFromApiAsync(url, code, currency, apiKey);
    }

    private async Task<CryptoPrice> FetchPriceFromApiAsync(string url, string code, string currency, string apiKey)
    {
        logger.LogInformation("Fetching price for {Code} from {URL}", code, url);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);

        var response = await client.GetAsync($"{url}?symbol={code}&convert={currency}");
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to get price from {URL}", url);
            throw new HttpRequestException("Failed to get price from the API");
        }

        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);
        var price = json["data"]?[code]?[0]?["quote"]?[currency]?["price"]?.Value<decimal>();
        if (price == null)
        {
            logger.LogError("Price not found in the API response");
            throw new KeyNotFoundException("Price not found in the API response");
        }

        logger.LogInformation("Fetched price for {Code} from {URL}", code, url);

        return new CryptoPrice
        {
            Code = code,
            BaseCurrency = currency,
            PriceBase = price.Value
        };
    }
}