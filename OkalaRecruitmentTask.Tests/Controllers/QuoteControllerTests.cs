using System.Configuration;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OkalaRecruitmentTask.Controllers;
using OkalaRecruitmentTask.Models;
using OkalaRecruitmentTask.Services;

namespace OkalaRecruitmentTask.Tests.Controllers;

public class QuoteControllerTests
{
    private readonly Mock<ICryptoQuoteService> _mockCryptoQuoteService;
    private readonly QuoteController _controller;
    
    public QuoteControllerTests()
    {
        Mock<ILogger<QuoteController>> mockLogger = new();
        _mockCryptoQuoteService = new Mock<ICryptoQuoteService>();
        _controller = new QuoteController(mockLogger.Object, _mockCryptoQuoteService.Object);
    }
    
    [Fact]
    public async Task GetQuoteAsync_WhenCodeIsEmpty_ReturnsBadRequest()
    {
        var result = await _controller.GetQuoteAsync(string.Empty);
        
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task GetQuoteAsync_WhenCodeIsValid_ReturnsQuote()
    {
        const string code = "BTC";
        var expectedQuote = new CryptoQuote
        {
            Code = code,
            Quote = new Dictionary<string, decimal> { { "USD", 50000 } }
        };
        _mockCryptoQuoteService.Setup(x => x.GetQuoteAsync("BTC")).ReturnsAsync(expectedQuote);
        
        var result = await _controller.GetQuoteAsync("BTC");
        
        Assert.IsType<CryptoQuote>(result.Value);
        Assert.Equal(expectedQuote, result.Value);
    }
    
    [Fact]
    public async Task GetQuoteAsync_WhenServiceThrowsHttpRequestException_ReturnsBadRequest()
    {
        _mockCryptoQuoteService.Setup(x => x.GetQuoteAsync(It.IsAny<string>())).ThrowsAsync(new HttpRequestException());
        
        var result = await _controller.GetQuoteAsync("BTC");
        
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task GetQuoteAsync_WhenServiceThrowsKeyNotFoundException_ReturnsNotFound()
    {
        _mockCryptoQuoteService.Setup(x => x.GetQuoteAsync(It.IsAny<string>())).ThrowsAsync(new KeyNotFoundException());
        
        var result = await _controller.GetQuoteAsync("BTC");
        
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
    
    [Fact]
    public async Task GetQuoteAsync_WhenServiceThrowsConfigurationErrorsException_ReturnsInternalServerError()
    {
        _mockCryptoQuoteService.Setup(x => x.GetQuoteAsync(It.IsAny<string>())).ThrowsAsync(new ConfigurationErrorsException());
        
        var result = await _controller.GetQuoteAsync("BTC");
        
        Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int) HttpStatusCode.InternalServerError, ((ObjectResult) result.Result).StatusCode);
    }
    
    [Fact]
    public async Task GetQuoteAsync_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        _mockCryptoQuoteService.Setup(x => x.GetQuoteAsync(It.IsAny<string>())).ThrowsAsync(new Exception());
        
        var result = await _controller.GetQuoteAsync("BTC");
        
        Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int) HttpStatusCode.InternalServerError, ((ObjectResult) result.Result).StatusCode);
    }
}