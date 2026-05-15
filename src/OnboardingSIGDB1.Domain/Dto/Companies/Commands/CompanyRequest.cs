namespace OnboardingSIGDB1.Domain.Dto.Companies.Commands;

public record CompanyRequest(
    string Name, 
    string Cnpj, 
    DateTime? FoundationDate
);