using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using OkalaRecruitmentTask.Configurations;
using OkalaRecruitmentTask.Services;

namespace OkalaRecruitmentTask.Tests.Services;

public class CoinMarketCapServiceTests
{
    private readonly Mock<IOptions<QuotesConfig>> _mockOptions;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly CoinMarketCapService _service;

    public CoinMarketCapServiceTests()
    {
        Mock<ILogger<CoinMarketCapService>> mockLogger = new();
        _mockOptions = new Mock<IOptions<QuotesConfig>>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new CoinMarketCapService(mockLogger.Object, _mockOptions.Object, httpClient);
    }

    private void SetupConfiguration(string baseCurrency = "USD", string apiUrl = "https://api.com",
        string apiKey = "api.key")
    {
        var configuration = new QuotesConfig
        {
            Currencies = new QuotesConfig.CurrenciesConfig {Base = baseCurrency},
            Apis = new QuotesConfig.ApisConfig
            {
                CoinMarketCap = new QuotesConfig.ApisConfig.CoinMarketCapConfig {Url = apiUrl, ApiKey = apiKey}
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

    private static string GetExpectedHttpResponse(decimal price, string baseCurrency = "USD", string code = "BTC")
    {
        var json = new JObject
        {
            ["data"] = new JObject
            {
                [code] = new JArray(new JObject
                    {["quote"] = new JObject {[baseCurrency] = new JObject {["price"] = price}}})
            }
        };
        return json.ToString();
    }

    [Fact]
    public async Task GetPriceAsync_WhenConfigurationIsValid_ReturnsPrice()
    {
        SetupConfiguration();
        SetupHttpClient(HttpStatusCode.OK, GetExpectedHttpResponse(50000));

        var price = await _service.GetPriceAsync("BTC");

        Assert.Equal(50000, price.PriceBase);
        Assert.Equal("USD", price.BaseCurrency);
        Assert.Equal("BTC", price.Code);
    }

    [Fact]
    public async Task GetPriceAsync_WhenApiReturnsError_ThrowsHttpRequestException()
    {
        SetupConfiguration();
        SetupHttpClient(HttpStatusCode.BadRequest);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetPriceAsync("BTC"));
        Assert.Equal("Failed to get price from the API", exception.Message);
    }

    [Fact]
    public async Task GetPriceAsync_WhenApiResponseIsInvalid_ThrowsKeyNotFoundException()
    {
        SetupConfiguration();
        SetupHttpClient(HttpStatusCode.OK, "{}");

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetPriceAsync("BTC"));
        Assert.Equal("Price not found in the API response", exception.Message);
    }
}