using Microsoft.Extensions.Options;

namespace OkalaRecruitmentTask.Configurations;

public class QuotesConfigValidator : IValidateOptions<QuotesConfig>
{
    public ValidateOptionsResult Validate(string? name, QuotesConfig config)
    {
        if (string.IsNullOrEmpty(config.Currencies.Base))
            return ValidateOptionsResult.Fail("Base currency not found in the configuration");

        if (config.Currencies.Required.Length == 0)
            return ValidateOptionsResult.Fail("At least one required currency symbol should be provided");

        if (config.Currencies.Required.Any(string.IsNullOrEmpty))
            return ValidateOptionsResult.Fail("Required currency symbol is empty");

        if (string.IsNullOrEmpty(config.Apis.CoinMarketCap.Url))
            return ValidateOptionsResult.Fail("CoinMarketCap API URL not found in the configuration");

        if (string.IsNullOrEmpty(config.Apis.CoinMarketCap.ApiKey))
            return ValidateOptionsResult.Fail("CoinMarketCap API key not found in the configuration");

        if (string.IsNullOrEmpty(config.Apis.ExchangeRates.Url))
            return ValidateOptionsResult.Fail("ExchangeRates API URL not found in the configuration");

        if (string.IsNullOrEmpty(config.Apis.ExchangeRates.ApiKey))
            return ValidateOptionsResult.Fail("ExchangeRates API key not found in the configuration");

        return ValidateOptionsResult.Success;
    }
}
