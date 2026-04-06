namespace OnboardingSIGDB1.Domain.Dto.Companies.Request;

public record CompanyRequest(
    string Name, 
    string Cnpj, 
    DateTime? FoundationDate
);