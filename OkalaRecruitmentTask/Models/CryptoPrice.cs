namespace OkalaRecruitmentTask.Models;

public class CryptoPrice
{
    public required string Code { get; set; }
    public required string BaseCurrency { get; set; }
    public required decimal PriceBase { get; set; }
}