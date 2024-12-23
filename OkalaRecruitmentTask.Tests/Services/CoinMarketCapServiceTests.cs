using System.Configuration;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using OkalaRecruitmentTask.Services;

namespace OkalaRecruitmentTask.Tests.Services;

public class CoinMarketCapServiceTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly CoinMarketCapService _service;

    public CoinMarketCapServiceTests()
    {
        Mock<ILogger<CoinMarketCapService>> mockLogger = new();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _service = new CoinMarketCapService(mockLogger.Object, _mockConfiguration.Object, httpClient);
    }

    private void SetupConfiguration(string? baseCurrency = "USD", string? apiUrl = "https://api.com",
        string? apiKey = "api.key")
    {
        _mockConfiguration.Setup(x => x["Quotes:Currencies:Base"]).Returns(baseCurrency);
        _mockConfiguration.Setup(x => x["Quotes:APIs:CoinMarketCap:URL"]).Returns(apiUrl);
        _mockConfiguration.Setup(x => x["Quotes:APIs:CoinMarketCap:APIKey"]).Returns(apiKey);
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
    public async Task GetPriceAsync_WhenBaseCurrencyIsMissing_ThrowsConfigurationErrorsException()
    {
        SetupConfiguration(baseCurrency: null);
        
        var exception = await Assert.ThrowsAsync<ConfigurationErrorsException>(() => _service.GetPriceAsync("BTC"));
        Assert.Equal("Base currency not found in the configuration", exception.Message);
    }
    
    [Fact]
    public async Task GetPriceAsync_WhenApiUrlIsMissing_ThrowsConfigurationErrorsException()
    {
        SetupConfiguration(apiUrl: null);
        
        var exception = await Assert.ThrowsAsync<ConfigurationErrorsException>(() => _service.GetPriceAsync("BTC"));
        Assert.Equal("CoinMarketCap API URL not found in the configuration", exception.Message);
    }
    
    [Fact]
    public async Task GetPriceAsync_WhenApiKeyIsMissing_ThrowsConfigurationErrorsException()
    {
        SetupConfiguration(apiKey: null);
        
        var exception = await Assert.ThrowsAsync<ConfigurationErrorsException>(() => _service.GetPriceAsync("BTC"));
        Assert.Equal("CoinMarketCap API key not found in the configuration", exception.Message);
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