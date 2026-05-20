namespace OnboardingSIGDB1.Domain.Dto.Companies.Commands;

/// <summary>
/// Request payload used to create or update a company.
/// </summary>
/// <param name="Name">Company legal name.</param>
/// <param name="Cnpj">Company registration number (CNPJ).</param>
/// <param name="FoundationDate">Company foundation date.</param>
public record CompanyRequest(
    string Name, 
    string Cnpj, 
    DateTime? FoundationDate
);