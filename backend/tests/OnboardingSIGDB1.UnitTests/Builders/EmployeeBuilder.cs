using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.UnitTests.Builders;

public class EmployeeBuilder
{
    private string _name = "John Doe";
    private string _cpf = "987.826.470-03";
    private DateTime _hireDate = DateTime.UtcNow.AddDays(-1);
    private int _companyId = 1;
    private int? _id;
    private Company? _company;

    public static EmployeeBuilder New() => new();

    public EmployeeBuilder WithName(string name) { _name = name; return this; }
    public EmployeeBuilder WithCpf(string cpf) { _cpf = cpf; return this; }
    public EmployeeBuilder WithHireDate(DateTime hireDate) { _hireDate = hireDate; return this; }
    public EmployeeBuilder WithCompanyId(int companyId) { _companyId = companyId; return this; }
    public EmployeeBuilder WithId(int id) { _id = id; return this; }
    public EmployeeBuilder WithCompany(Company company) { _company = company; return this; }

    public Employee Build()
    {
        var employee = new Employee(_name, _cpf, _hireDate, _companyId);

        if (_id.HasValue)
        {
            var prop = employee.GetType().GetProperty("Id");
            prop?.SetValue(employee, _id.Value);
        }

        if (_company != null)
        {
            var prop = employee.GetType().GetProperty("Company");
            prop?.SetValue(employee, _company);
        }

        return employee;
    }
}

