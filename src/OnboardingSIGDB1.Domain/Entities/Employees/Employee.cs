using OnboardingSIGDB1.Domain.Base;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Entities.Positions;
using OnboardingSIGDB1.Domain.Utils;

namespace OnboardingSIGDB1.Domain.Entities;

public class Employee : BaseEntity
{
    public string Name { get; private set; }
    public string Cpf { get; private set; }
    public DateTime? HireDate { get; private set; }
    
    public int CompanyId { get; private set; }
    public Company Company { get; private set; }
    
    private readonly List<EmployeePosition> _positions = new();
    public IReadOnlyCollection<EmployeePosition> Positions => _positions.AsReadOnly();

    protected Employee(){}
    
    public Employee(string name, string cpf, DateTime? hireDate, Company company)
    {
        SetName(name);
        SetCpf(cpf);
        SetHireDate(hireDate);
        SetCompany(company);
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string newName) => SetName(newName);

    public void AddPosition(Position position, DateTime startDate)
    {
        if(CompanyId == 0)
            AddNotification("Company", "Employee must be linked to a company before assigning a position.");
        
        if(position is null)
            AddNotification("Position", "Position is required.");
        
        if(startDate == DateTime.MinValue)
            AddNotification("StartDate", "Start date is required.");
        
        if(startDate > DateTime.UtcNow)
            AddNotification("StartDate", "Start date cannot be in the future.");
        
        if (position != null && _positions.Any(ep => ep.PositionId == position.Id))
            AddNotification("Position", "Employee already has this position.");

        if (!IsValid)
            return;
            
        _positions.Add(new EmployeePosition(this, position, startDate));
    }

    public Position? GetLastPosition()
    {
        return _positions
            .OrderByDescending(ep => ep.StartDate)
            .Select(ep => ep.Position)
            .FirstOrDefault();
    } 
    
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            AddNotification("Name", "Employee name is required.");
        else if (name.Length > 150)
            AddNotification("Name", "Employee name must not exceed 150 characters.");
        else
            Name = name.Trim();
    }
    
    private void SetCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            AddNotification("Cpf", "CPF is required.");
            return;
        }

        if (!CpfValidator.IsValid(cpf))
        {
            AddNotification("Cpf", "Invalid CPF.");
            return;
        }

        Cpf = StringUtils.OnlyNumbers(cpf);
    }

    private void SetHireDate(DateTime? hireDate)
    {
        if (hireDate.HasValue && hireDate > DateTime.UtcNow)
            AddNotification("HireDate", "Hire date cannot be in the future.");
        else
            HireDate = hireDate;
    }
    
    private void SetCompany(Company company)
    {
        if (company is null)
        {
            AddNotification("Company", "Company is required.");
            return;
        }

        if (CompanyId != 0)
        {
            AddNotification("Company", "Employee is already linked to a company.");
            return;
        }
        
        Company = company;
        CompanyId = company.Id;
    }
}