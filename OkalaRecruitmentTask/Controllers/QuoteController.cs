using Microsoft.AspNetCore.Mvc;
using OkalaRecruitmentTask.Models;
using OkalaRecruitmentTask.Services;
using System.Configuration;
using System.Net;

namespace OkalaRecruitmentTask.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuoteController(ILogger<QuoteController> logger, ICryptoQuoteService cryptoQuoteService) : ControllerBase
{
    [HttpGet("{code}")]
    public async Task<ActionResult<CryptoQuote>> GetQuoteAsync(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            logger.LogError("Code is required");
            return BadRequest("Code is required");
        }

        logger.LogInformation("Received a new quote request for {Code}", code);

        try
        {
            var quote = await cryptoQuoteService.GetQuoteAsync(code.ToUpper());
            logger.LogInformation("Successfully got quote for {Code}, sending response", code);
            return quote;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "Failed to get quote for {Code}", code);
            return BadRequest("Failed to get quote for the code");
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, "Failed to get quote for {Code}", code);
            return NotFound("Failed to get quote for the code");
        }
        catch (ConfigurationErrorsException ex)
        {
            logger.LogCritical(ex, "Configuration error");
            return StatusCode((int) HttpStatusCode.InternalServerError, "Failed to get quote for the code");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unknown error occurred while getting quote for {Code}", code);
            return StatusCode((int) HttpStatusCode.InternalServerError,
                "An unknown error occurred while getting quote for the code");
        }
    }
}