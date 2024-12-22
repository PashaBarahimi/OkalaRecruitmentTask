using Microsoft.AspNetCore.Mvc;
using OkalaRecruitmentTask.Models;
using OkalaRecruitmentTask.Services;

namespace OkalaRecruitmentTask.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuoteController(ILogger<QuoteController> logger, ICryptoQuoteService cryptoQuoteService) : ControllerBase
{
    public readonly ILogger<QuoteController> _logger = logger;
    private readonly ICryptoQuoteService _cryptoQuoteService = cryptoQuoteService;

    [HttpGet("{code}")]
    public async Task<ActionResult<CryptoQuote>> GetQuoteAsync(string code)
    {
        _logger.LogInformation("Getting quote for {Code}", code);
        var quote = await _cryptoQuoteService.GetQuoteAsync(code);
        if (quote is null)
        {
            _logger.LogWarning("Quote for {Code} not found", code);
            return NotFound();
        }
        return quote;
    }
}
