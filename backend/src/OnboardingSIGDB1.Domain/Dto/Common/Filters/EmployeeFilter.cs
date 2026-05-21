
namespace OnboardingSIGDB1.Domain.Dto.Common.Filters;

/// <summary>
/// Query filter used to search employees.
/// </summary>
public class EmployeeFilter : BaseFilter
{
    /// <summary>
    /// Filters employees by name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Filters employees by CPF.
    /// </summary>
    public string? Cpf { get; set; }

    /// <summary>
    /// Returns employees hired on or after this date.
    /// </summary>
    public DateTime? HiredFrom { get; set; }

    /// <summary>
    /// Returns employees hired on or before this date.
    /// </summary>
    public DateTime? HiredUntil { get; set; }
}