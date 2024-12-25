namespace OkalaRecruitmentTask.Configurations;

public class QuotesConfig
{
    public CurrenciesConfig? Currencies { get; init; }
    public ApisConfig? Apis { get; init; }

    public class CurrenciesConfig
    {
        public string? Base { get; init; }
        public string[]? Required { get; init; }
    }

    public class ApisConfig
    {
        public ExchangeRatesConfig? ExchangeRates { get; init; }
        public CoinMarketCapConfig? CoinMarketCap { get; init; }

        public class ExchangeRatesConfig
        {
            public string? Url { get; init; }
            public string? ApiKey { get; init; }
        }

        public class CoinMarketCapConfig
        {
            public string? Url { get; init; }
            public string? ApiKey { get; init; }
        }
    }
}