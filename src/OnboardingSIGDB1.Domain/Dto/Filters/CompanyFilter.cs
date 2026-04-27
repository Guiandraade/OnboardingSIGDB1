
namespace OnboardingSIGDB1.Domain.Dto.Filters;

public class CompanyFilter : BaseFilter
{
    public string? Name { get; set; }
    public string? Cnpj { get; set; }
    public DateTime? FoundedIn { get; set; }
    public DateTime? FoundedUntil { get; set; }
}