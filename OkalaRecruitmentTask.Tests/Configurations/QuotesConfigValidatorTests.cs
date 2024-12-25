using OkalaRecruitmentTask.Configurations;

namespace OkalaRecruitmentTask.Tests.Configurations;

public class QuotesConfigValidatorTests
{
    private readonly QuotesConfigValidator _validator = new();

    [Fact]
    public void Validate_WhenConfigurationIsValid_ValidatesSuccessfully()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = ["EUR"]},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                },
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void Validate_WhenBaseCurrencyIsMissing_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig(),
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("Base currency not found in the configuration", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenRequiredCurrencyListIsEmpty_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = []},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                },
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("At least one required currency symbol should be provided", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenRequiredCurrencyListContainsEmptyString_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = [""]},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                },
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("Required currency symbol is empty", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenRequiredCurrencyListIsNull_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD"},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                },
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("Required currency symbol is empty", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenCurrenciesConfigIsNull_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                },
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("Base currency not found in the configuration", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenCoinMarketCapUrlIsMissing_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = ["EUR"]},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    ApiKey = "api.key"
                },
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("CoinMarketCap API URL not found in the configuration", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenCoinMarketCapApiKeyIsMissing_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = ["EUR"]},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com"
                },
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("CoinMarketCap API key not found in the configuration", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenCoinMarketCapConfigIsNull_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = ["EUR"]},
            Apis = new QuotesConfig.ApisConfig
            {
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("CoinMarketCap API URL not found in the configuration", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenExchangeRatesUrlIsMissing_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = ["EUR"]},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                },
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("ExchangeRates API URL not found in the configuration", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenExchangeRatesApiKeyIsMissing_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = ["EUR"]},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                },
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig
                {
                    Url = "https://api.com"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("ExchangeRates API key not found in the configuration", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenExchangeRatesConfigIsNull_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = ["EUR"]},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig
                {
                    Url = "https://api.com",
                    ApiKey = "api.key"
                }
            }
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("ExchangeRates API URL not found in the configuration", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenApisConfigIsNull_FailsValidation()
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = "USD", Required = ["EUR"]}
        };

        var result = _validator.Validate(null, configuration);

        Assert.True(result.Failed);
        Assert.Equal("CoinMarketCap API URL not found in the configuration", result.FailureMessage);
    }
    
    [Fact]
    public void Validate_WhenConfigurationIsNull_FailsValidation()
    {
        var result = _validator.Validate(null, new QuotesConfig());

        Assert.True(result.Failed);
        Assert.Equal("Base currency not found in the configuration", result.FailureMessage);
    }
}