namespace OnboardingSIGDB1.Domain.Dto.Companies.Responses;

/// <summary>
/// Response payload representing a company.
/// </summary>
public record CompanyResponse
{
    /// <summary>
    /// Company identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Company legal name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Company registration number (CNPJ).
    /// </summary>
    public string Cnpj { get; init; } = string.Empty;

    /// <summary>
    /// Company foundation date.
    /// </summary>
    public DateTime? FoundationDate { get; init; }
}