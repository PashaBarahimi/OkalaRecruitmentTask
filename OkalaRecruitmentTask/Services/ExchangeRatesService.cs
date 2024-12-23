using OkalaRecruitmentTask.Models;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace OkalaRecruitmentTask.Services;

public class ExchangeRatesService(ILogger<ExchangeRatesService> logger, IConfiguration configuration) : ICurrencyRatesService
{
    public async Task<CurrencyRates> GetCurrencyRatesAsync()
    {
        logger.LogInformation("Getting currency rates");

        var baseCurrency = configuration["Quotes:Currencies:Base"];
        if (string.IsNullOrEmpty(baseCurrency))
        {
            logger.LogCritical("Base currency not found in the configuration");
            throw new ConfigurationErrorsException("Base currency not found in the configuration");
        }

        return await GetCurrencyRatesWithBaseAsync(baseCurrency);
    }

    private async Task<CurrencyRates> GetCurrencyRatesWithBaseAsync(string baseCurrency)
    {
        logger.LogInformation("Getting currency rates with base {BaseCurrency}", baseCurrency);

        var url = configuration["Quotes:APIs:ExchangeRates:URL"];
        if (string.IsNullOrEmpty(url))
        {
            logger.LogCritical("ExchangeRates API URL not found in the configuration");
            throw new ConfigurationErrorsException("ExchangeRates API URL not found in the configuration");
        }

        var apiKey = configuration["Quotes:APIs:ExchangeRates:APIKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            logger.LogCritical("ExchangeRates API key not found in the configuration (CAUTION: Use dotnet user-secrets)");
            throw new ConfigurationErrorsException("ExchangeRates API key not found in the configuration");
        }

        var requiredSymbols = configuration.GetSection("Quotes:Currencies:Required").Get<string[]>();
        if (requiredSymbols is null || requiredSymbols.Length == 0)
        {
            logger.LogCritical("Required currencies not found in the configuration");
            throw new ConfigurationErrorsException("Required currencies not found in the configuration");
        }
        var symbols = string.Join(',', requiredSymbols);

        return await FetchCurrencyRatesFromApiAsync(url, baseCurrency, symbols, apiKey);
    }

    private async Task<CurrencyRates> FetchCurrencyRatesFromApiAsync(string url, string baseCurrency, string symbols, string apiKey)
    {
        logger.LogInformation("Fetching currency rates from {URL}", url);

        using var client = new HttpClient();
        var response = await client.GetAsync($"{url}?base={baseCurrency}&symbols={symbols}&access_key={apiKey}");
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Failed to get currency rates from {URL}", url);
            throw new HttpRequestException("Failed to get currency rates from the API");
        }

        var content = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(content);
        var rates = json["rates"]?.ToObject<Dictionary<string, decimal>>();
        if (rates is null || rates.Count == 0)
        {
            logger.LogError("Currency rates not found in the API response");
            throw new KeyNotFoundException("Currency rates not found in the API response");
        }

        logger.LogInformation("Fetched currency rates from {URL}", url);

        return new CurrencyRates
        {
            BaseCurrency = baseCurrency,
            Rates = rates
        };
    }
}