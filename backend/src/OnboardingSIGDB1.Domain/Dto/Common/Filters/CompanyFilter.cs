
namespace OnboardingSIGDB1.Domain.Dto.Common.Filters;

/// <summary>
/// Query filter used to search companies.
/// </summary>
public class CompanyFilter : BaseFilter
{
    /// <summary>
    /// Filters companies by name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filters companies by CNPJ.
    /// </summary>
    public string? Cnpj { get; set; }

    /// <summary>
    /// Returns companies founded on or after this date.
    /// </summary>
    public DateTime? FoundedIn { get; set; }

    /// <summary>
    /// Returns companies founded on or before this date.
    /// </summary>
    public DateTime? FoundedUntil { get; set; }
}