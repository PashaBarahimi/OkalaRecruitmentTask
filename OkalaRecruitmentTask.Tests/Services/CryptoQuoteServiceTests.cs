using Microsoft.Extensions.Logging;
using Moq;
using OkalaRecruitmentTask.Models;
using OkalaRecruitmentTask.Services;

namespace OkalaRecruitmentTask.Tests.Services;

public class CryptoQuoteServiceTests
{
    private readonly Mock<ICryptoPriceService> _mockCryptoPriceService;
    private readonly Mock<ICurrencyRatesService> _mockCurrencyRatesService;
    private readonly CryptoQuoteService _service;

    public CryptoQuoteServiceTests()
    {
        Mock<ILogger<CryptoQuoteService>> loggerMock = new();
        _mockCryptoPriceService = new Mock<ICryptoPriceService>();
        _mockCurrencyRatesService = new Mock<ICurrencyRatesService>();
        _service = new CryptoQuoteService(_mockCryptoPriceService.Object, _mockCurrencyRatesService.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task GetQuoteAsync_WhenDataIsValid_ReturnsQuote()
    {
        const string code = "BTC";
        var price = new CryptoPrice {PriceBase = 1000, Code = "BTC", BaseCurrency = "EUR"};
        var currencyRates = new CurrencyRates
            {Rates = new Dictionary<string, decimal> {{"USD", 1.2m}, {"EUR", 1m}}, BaseCurrency = "EUR"};
        _mockCryptoPriceService.Setup(x => x.GetPriceAsync(code)).ReturnsAsync(price);
        _mockCurrencyRatesService.Setup(x => x.GetCurrencyRatesAsync()).ReturnsAsync(currencyRates);

        var result = await _service.GetQuoteAsync(code);

        Assert.Equal(code, result.Code);
        Assert.Equal(1200, result.Quote["USD"]);
        Assert.Equal(1000, result.Quote["EUR"]);
    }

    [Fact]
    public async Task GetQuoteAsync_WhenPriceServiceFailed_ThrowsException()
    {
        const string code = "BTC";
        _mockCryptoPriceService.Setup(x => x.GetPriceAsync(code)).ThrowsAsync(new Exception("Failed to fetch price"));

        var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.GetQuoteAsync(code));
        Assert.Equal("Failed to fetch price", exception.Message);
    }

    [Fact]
    public async Task GetQuoteAsync_WhenCurrencyRatesServiceFailed_ThrowsException()
    {
        const string code = "BTC";
        _mockCryptoPriceService.Setup(x => x.GetPriceAsync(code)).ReturnsAsync(new CryptoPrice
            {PriceBase = 1000, Code = "BTC", BaseCurrency = "EUR"});
        _mockCurrencyRatesService.Setup(x => x.GetCurrencyRatesAsync())
            .ThrowsAsync(new Exception("Failed to fetch currency rates"));

        var exception = await Assert.ThrowsAsync<Exception>(async () => await _service.GetQuoteAsync(code));
        Assert.Equal("Failed to fetch currency rates", exception.Message);
    }
}