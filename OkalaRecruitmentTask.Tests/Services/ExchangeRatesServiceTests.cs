using System.Configuration;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using OkalaRecruitmentTask.Services;

namespace OkalaRecruitmentTask.Tests.Services;

public class ExchangeRatesServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly ExchangeRatesService _service;

    public ExchangeRatesServiceTests()
    {
        Mock<ILogger<ExchangeRatesService>> mockLogger = new();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new ExchangeRatesService(mockLogger.Object, _mockConfiguration.Object, httpClient);
    }

    private void SetupConfiguration(string? baseCurrency = "EUR", string? apiUrl = "https://api.com",
        string? apiKey = "api.key", string[]? requiredSymbols = null)
    {
        _mockConfiguration.Setup(x => x["Quotes:Currencies:Base"]).Returns(baseCurrency);
        _mockConfiguration.Setup(x => x["Quotes:APIs:ExchangeRates:URL"]).Returns(apiUrl);
        _mockConfiguration.Setup(x => x["Quotes:APIs:ExchangeRates:APIKey"]).Returns(apiKey);

        List<IConfigurationSection> children = [];
        foreach (var symbol in requiredSymbols ?? ["USD", "EUR"])
        {
            Mock<IConfigurationSection> mockSection = new();
            mockSection.Setup(x => x.Value).Returns(symbol);
            children.Add(mockSection.Object);
        }
        
        Mock<IConfigurationSection> mockRequiredSection = new();
        mockRequiredSection.Setup(x => x.GetChildren()).Returns(children);
        _mockConfiguration.Setup(x => x.GetSection("Quotes:Currencies:Required")).Returns(mockRequiredSection.Object);
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
    public async Task GetCurrencyRatesAsync_WhenBaseCurrencyIsMissing_ThrowsConfigurationErrorsException()
    {
        SetupConfiguration(baseCurrency: null);

        var exception = await Assert.ThrowsAsync<ConfigurationErrorsException>(async () => await _service.GetCurrencyRatesAsync());
        Assert.Equal("Base currency not found in the configuration", exception.Message);
    }
    
    [Fact]
    public async Task GetCurrencyRatesAsync_WhenApiUrlIsMissing_ThrowsConfigurationErrorsException()
    {
        SetupConfiguration(apiUrl: null);

        var exception = await Assert.ThrowsAsync<ConfigurationErrorsException>(async () => await _service.GetCurrencyRatesAsync());
        Assert.Equal("ExchangeRates API URL not found in the configuration", exception.Message);
    }
    
    [Fact]
    public async Task GetCurrencyRatesAsync_WhenApiKeyIsMissing_ThrowsConfigurationErrorsException()
    {
        SetupConfiguration(apiKey: null);

        var exception = await Assert.ThrowsAsync<ConfigurationErrorsException>(async () => await _service.GetCurrencyRatesAsync());
        Assert.Equal("ExchangeRates API key not found in the configuration", exception.Message);
    }
    
    [Fact]
    public async Task GetCurrencyRatesAsync_WhenRequiredSymbolsAreMissing_ThrowsConfigurationErrorsException()
    {
        SetupConfiguration(requiredSymbols: []);

        var exception = await Assert.ThrowsAsync<ConfigurationErrorsException>(async () => await _service.GetCurrencyRatesAsync());
        Assert.Equal("Required currencies not found in the configuration", exception.Message);
    }
    
    [Fact]
    public async Task GetCurrencyRatesAsync_WhenApiRequestFailed_ThrowsHttpRequestException()
    {
        SetupConfiguration();
        SetupHttpClient(HttpStatusCode.BadRequest);

        var exception = await Assert.ThrowsAsync<HttpRequestException>(async () => await _service.GetCurrencyRatesAsync());
        Assert.Equal("Failed to get currency rates from the API", exception.Message);
    }
    
    [Fact]
    public async Task GetCurrencyRatesAsync_WhenApiResponseIsInvalid_ThrowsKeyNotFoundException()
    {
        SetupConfiguration();
        SetupHttpClient(HttpStatusCode.OK, "{}");

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(async () => await _service.GetCurrencyRatesAsync());
        Assert.Equal("Currency rates not found in the API response", exception.Message);
    }
}