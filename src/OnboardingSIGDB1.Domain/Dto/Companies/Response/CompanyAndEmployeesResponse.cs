namespace OnboardingSIGDB1.Domain.Dto.Companies.Response;

public record CompanyAndEmployeesResponse(
    int Id,
    string Name,
    string Cnpj,
    DateTime? FoundationDate,
    List<CompanyDetailsResponse> EmployeesPositionHistory
);