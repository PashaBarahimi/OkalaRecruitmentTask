namespace OkalaRecruitmentTask.Models;

public class ExchangeRates
{
    public required string BaseCurrency { get; set; }
    public required Dictionary<string, decimal> Rates { get; set; }
}
