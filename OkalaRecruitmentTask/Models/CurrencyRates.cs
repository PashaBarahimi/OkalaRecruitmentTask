namespace OkalaRecruitmentTask.Models;

public class CurrencyRates
{
    public required string BaseCurrency { get; set; }
    public required Dictionary<string, decimal> Rates { get; init; }
}
