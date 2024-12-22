using OkalaRecruitmentTask.Models;

namespace OkalaRecruitmentTask.Services
{
    public class CryptoPriceService(ILogger<CryptoPriceService> logger) : ICryptoPriceService
    {
        private readonly ILogger<CryptoPriceService> _logger = logger;

        public Task<CryptoPrice?> GetPriceAsync(string code)
        {
            _logger.LogInformation("Getting price for {Code}", code);

            // This is a placeholder for a real API call
            var price = new CryptoPrice
            {
                Code = code,
                PriceBase = 50000.0m
            };

            _logger.LogInformation("Price for {Code} found", code);

            return Task.FromResult<CryptoPrice?>(price);
        }
    }
}
