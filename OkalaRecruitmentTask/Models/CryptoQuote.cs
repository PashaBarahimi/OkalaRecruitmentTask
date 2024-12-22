namespace OkalaRecruitmentTask.Models;

public class CryptoQuote
{
    public required string Code { get; set; }
    public required Dictionary<string, decimal> Quote { get; set; }
}
