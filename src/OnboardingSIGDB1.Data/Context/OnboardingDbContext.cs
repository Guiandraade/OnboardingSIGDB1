using Microsoft.EntityFrameworkCore;
using OnboardingSIGDB1.Domain.Entities;
using OnboardingSIGDB1.Domain.Entities.Employees;
using OnboardingSIGDB1.Domain.Entities.Companies;
using OnboardingSIGDB1.Domain.Entities.Positions;

using OnboardingSIGDB1.Domain.Notifications;

namespace OnboardingSIGDB1.Data;

public class OnboardingDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public OnboardingDbContext(DbContextOptions<OnboardingDbContext> options) : base(options){ }
    
    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees  { get; set; }
    public DbSet<EmployeePosition> EmployeePositions { get; set; }
    public DbSet<Position> Positions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Notification>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyMap).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}