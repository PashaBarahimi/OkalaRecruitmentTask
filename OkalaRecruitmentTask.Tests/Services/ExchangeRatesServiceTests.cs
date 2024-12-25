using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using OkalaRecruitmentTask.Configurations;
using OkalaRecruitmentTask.Services;

namespace OkalaRecruitmentTask.Tests.Services;

public class ExchangeRatesServiceTests
{
    private readonly Mock<IOptions<QuotesConfig>> _mockOptions;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly ExchangeRatesService _service;

    public ExchangeRatesServiceTests()
    {
        Mock<ILogger<ExchangeRatesService>> mockLogger = new();
        _mockOptions = new Mock<IOptions<QuotesConfig>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new ExchangeRatesService(mockLogger.Object, _mockOptions.Object, httpClient);
    }

    private void SetupConfiguration(string baseCurrency = "EUR", string apiUrl = "https://api.com",
        string apiKey = "api.key", string[]? requiredSymbols = null)
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig
            {
                Base = baseCurrency, Required = requiredSymbols ?? ["USD", "EUR"]
            },
            Apis = new QuotesConfig.ApisConfig
            {
                ExchangeRates = new QuotesConfig.ApisConfig.ExchangeRatesConfig {Url = apiUrl, ApiKey = apiKey}
            }
        };
        _mockOptions.Setup(x => x.Value).Returns(configuration);
    }

    private void SetupHttpClient(HttpStatusCode statusCode = HttpStatusCode.OK, string content = "")
    {
        var response = new HttpResponseMessage(statusCode) {Content = new StringContent(content)};
        _mockHttpMessageHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
    }

    private static string GetExpectedHttpResponse(Dictionary<string, decimal>? rates)
    {
        if (rates is null)
            return string.Empty;
        var json = new JObject {["rates"] = new JObject()};
        foreach (var (key, value) in rates)
        {
            json["rates"]![key] = value;
        }

        return json.ToString();
    }

    [Fact]
    public async Task GetCurrencyRatesAsync_WhenConfigurationIsValid_ReturnsCurrencyRates()
    {
        var requiredSymbols = new[] {"USD", "EUR"};
        var rates = new Dictionary<string, decimal> {{"USD", 1.2m}, {"EUR", 1m}};
        SetupConfiguration(requiredSymbols: requiredSymbols);
        SetupHttpClient(HttpStatusCode.OK, GetExpectedHttpResponse(rates));

        var result = await _service.GetCurrencyRatesAsync();

        Assert.Equal("EUR", result.BaseCurrency);
        Assert.Equal(rates, result.Rates);
    }

    [Fact]
    public async Task GetCurrencyRatesAsync_WhenApiRequestFailed_ThrowsHttpRequestException()
    {
        SetupConfiguration();
        SetupHttpClient(HttpStatusCode.BadRequest);

        var exception =
            await Assert.ThrowsAsync<HttpRequestException>(async () => await _service.GetCurrencyRatesAsync());
        Assert.Equal("Failed to get currency rates from the API", exception.Message);
    }

    [Fact]
    public async Task GetCurrencyRatesAsync_WhenApiResponseIsInvalid_ThrowsKeyNotFoundException()
    {
        SetupConfiguration();
        SetupHttpClient(HttpStatusCode.OK, "{}");

        var exception =
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetCurrencyRatesAsync());
        Assert.Equal("Currency rates not found in the API response", exception.Message);
    }
}