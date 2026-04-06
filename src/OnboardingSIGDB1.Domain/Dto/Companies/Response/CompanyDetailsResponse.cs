namespace OnboardingSIGDB1.Domain.Dto.Companies.Response;

public class CompanyDetailsResponse
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string PositionName { get; set; }
    public DateTime HiringDate { get; set; }
}