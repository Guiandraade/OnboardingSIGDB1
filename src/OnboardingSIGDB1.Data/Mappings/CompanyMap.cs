using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnboardingSIGDB1.Domain.Entities.Companies;

namespace OnboardingSIGDB1.Data;

public class CompanyMap : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("Companies");
        
        builder.HasKey(C => C.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        
        builder.Property(C => C.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Cnpj)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(c => c.FoundationDate).IsRequired(false);

        builder.Property(c => c.CreatedAt)
            .IsRequired();
        
        builder.HasMany(c => c.Employees)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Navigation(c => c.Employees)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
    
}