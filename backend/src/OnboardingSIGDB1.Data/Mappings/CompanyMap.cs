using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnboardingSIGDB1.Domain.Entities.Companies;

namespace OnboardingSIGDB1.Data.Mappings;

public class CompanyMap : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        // Table & Key
        builder.ToTable("Companies");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();

        // Properties
        builder.Property(c => c.Name).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Cnpj).IsRequired().HasMaxLength(14);
        builder.Property(c => c.FoundationDate).IsRequired(false);
        builder.Property(c => c.CreatedAt).IsRequired();

        // Ignore validation
        builder.Ignore(c => c.ValidationResult);

        // Relationships
        builder.HasMany(c => c.Employees)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation
        builder.Navigation(c => c.Employees)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
    
}