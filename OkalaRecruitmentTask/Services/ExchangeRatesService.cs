using OkalaRecruitmentTask.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;
using OkalaRecruitmentTask.Configurations;

namespace OkalaRecruitmentTask.Services;

public class ExchangeRatesService(
    ILogger<ExchangeRatesService> logger,
    IOptions<QuotesConfig> configuration,
    HttpClient client)
    : ICurrencyRatesService
{
    public async Task<CurrencyRates> GetCurrencyRatesAsync()
    {
        logger.LogInformation("Getting currency rates");

        var baseCurrency = configuration.Value.Currencies!.Base!;

        return await GetCurrencyRatesWithBaseAsync(baseCurrency);
    }

    private async Task<CurrencyRates> GetCurrencyRatesWithBaseAsync(string baseCurrency)
    {
        logger.LogInformation("Getting currency rates with base {BaseCurrency}", baseCurrency);

        var url = configuration.Value.Apis!.ExchangeRates.Url!;
        var apiKey = configuration.Value.Apis.ExchangeRates.ApiKey!;
        var requiredSymbols = configuration.Value.Currencies!.Required!;
        var symbols = string.Join(',', requiredSymbols);

        return await FetchCurrencyRatesFromApiAsync(url, baseCurrency, symbols, apiKey);
    }

    private async Task<CurrencyRates> FetchCurrencyRatesFromApiAsync(string url, string baseCurrency, string symbols,
        string apiKey)
    {
        logger.LogInformation("Fetching currency rates from {URL}", url);

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