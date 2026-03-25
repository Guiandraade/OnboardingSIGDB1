using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.Domain.Entities.Companies;

public class Company : BaseEntity
{ 
    public string Name { get; private set; }
    public string Cnpj { get; private set; }
    public DateTime? FoundationDate  { get; private set; }

    private readonly List<Employee> _employees = new();
    public IReadOnlyCollection<Employee> Employees => _employees.AsReadOnly();
    
    protected Company() { }

    public Company(string name, string  cnpj, DateTime? foundationDate)
    {
        SetName(name);
        SetCnpj(cnpj);
        SetFoundationDate(foundationDate);
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string name) => SetName(name);
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            AddNotification("Name", "Name is required.");
        
        else if (name.Length > 150)
            AddNotification("Name", "Company name must not exceed 150 characters.");
        
        else
            Name = name.Trim();
    }

    private void SetCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
        {
            AddNotification("Cnpj", "CNPJ is required.");
            return;
        }
        
        if(!CnpjValidator.IsValid(cnpj))
        {
            AddNotification("Cnpj", "Invalid CNPJ.");
            return;
        }

        Cnpj = StringUtils.OnlyNumbers(cnpj);
    }

    private void SetFoundationDate(DateTime? foundationDate)
    {
        if(foundationDate.HasValue && foundationDate > DateTime.UtcNow)
            AddNotification("FoundationDate", "Foundation date cannot be in the future.");
        else
            FoundationDate = foundationDate;
    }
}