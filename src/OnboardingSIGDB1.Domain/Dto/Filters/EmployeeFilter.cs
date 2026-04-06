using OnboardingSIGDB1.Domain.Dto.filters;

namespace OnboardingSIGDB1.Domain.Dto.Filters;

public class EmployeeFilter : BaseFilter
{
    public string? Name { get; set; }
    public string? Cpf { get; set; }
    public DateTime? HiredFrom { get; set; }
    public DateTime? HiredUntil { get; set; }
}