using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.UnitTests.Builders;

public class EmployeeBuilder
{
    private string _name = "Guilherme";
    private string _cpf = "987.826.470-03";
    private DateTime? _hireDate = DateTime.UtcNow.AddDays(-1);
    private int _companyId = 1;

    public static EmployeeBuilder New() => new();

    public EmployeeBuilder WithName(string name) { _name = name; return this; }
    public EmployeeBuilder WithCpf(string cpf) { _cpf = cpf; return this; }
    public EmployeeBuilder WithHireDate(DateTime? hireDate) { _hireDate = hireDate; return this; }
    public EmployeeBuilder WithCompanyId(int companyId) { _companyId = companyId; return this; }

    public Employee Build() => new(_name, _cpf, _hireDate, _companyId);
}

