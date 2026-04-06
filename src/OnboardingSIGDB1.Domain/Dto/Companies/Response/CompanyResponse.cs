namespace OnboardingSIGDB1.Domain.Dto.Companies.Response;

public record CompanyResponse(
    int Id, 
    string Name, 
    string Cnpj, 
    DateTime? FoundationDate
);