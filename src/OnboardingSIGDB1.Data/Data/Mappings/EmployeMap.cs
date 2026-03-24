using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnboardingSIGDB1.Domain.Entities;

namespace OnboardingSIGDB1.Infrastructure.Data.Mappings;

public class EmployeMap : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(e => e.HireDate);

        builder.Property(e => e.CreatedAt)
            .IsRequired();
        
        builder.HasOne(e => e.Company)
            .WithMany(e => e.Employees)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(e => e.Positions)
            .WithOne(e => e.Employee)
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Navigation(e => e.Positions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}