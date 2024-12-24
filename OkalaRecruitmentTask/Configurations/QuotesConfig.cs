namespace OkalaRecruitmentTask.Configurations;

public class QuotesConfig
{
    public required CurrenciesConfig Currencies { get; init; }
    public required ApisConfig Apis { get; init; }

    public class CurrenciesConfig
    {
        public required string Base { get; init; }
        public required string[] Required { get; init; }
    }

    public class ApisConfig
    {
        public required ExchangeRatesConfig ExchangeRates { get; init; }
        public required CoinMarketCapConfig CoinMarketCap { get; init; }

        public class ExchangeRatesConfig
        {
            public required string Url { get; init; }
            public required string ApiKey { get; init; }
        }

        public class CoinMarketCapConfig
        {
            public required string Url { get; init; }
            public required string ApiKey { get; init; }
        }
    }
}