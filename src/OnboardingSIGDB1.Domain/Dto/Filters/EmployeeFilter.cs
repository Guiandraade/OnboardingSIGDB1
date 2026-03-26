namespace OnboardingSIGDB1.Domain.Dto.filters;

public class EmployeeFilter : BaseFilter
{
    public string? Name { get; set; }
    public string? Cpf { get; set; }
    public DateTime? HiredFrom { get; set; }
    public DateTime? Hireduntil { get; set; }
}