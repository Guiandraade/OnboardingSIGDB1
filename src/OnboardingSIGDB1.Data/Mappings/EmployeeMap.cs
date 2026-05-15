using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Data.Mappings;

public class EmployeeMap : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        // Table & Primary Key
        builder.ToTable("Employees");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        // Properties
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Cpf)
            .IsRequired()
            .HasMaxLength(11);

        builder.HasIndex(e => e.Cpf)
            .IsUnique();

        builder.Property(e => e.HireDate).IsRequired(false);
        builder.Property(e => e.CreatedAt).IsRequired();

        // Ignored properties
        builder.Ignore(e => e.ValidationResult);

        // Relationships
        builder.HasOne(e => e.Company)
            .WithMany(e => e.Employees)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation settings
        builder.Navigation(e => e.Positions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}