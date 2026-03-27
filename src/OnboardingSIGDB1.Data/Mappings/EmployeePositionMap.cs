using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnboardingSIGDB1.Domain.Entities.Employees;

namespace OnboardingSIGDB1.Data;

public class EmployeePositionMap : IEntityTypeConfiguration<EmployeePosition>
{
    public void Configure(EntityTypeBuilder<EmployeePosition> builder)
    {
        builder.ToTable("EmployeePositions");
        builder.HasKey(e => new { e.EmployeeId, e.PositionId });

        builder.Property(e => e.StartDate).IsRequired();
        
        builder.Property(e => e.CreatedAt).IsRequired();
        
        builder.Ignore(e => e.ValidationResult);
        
        builder.HasOne(e => e.Employee)
            .WithMany(e => e.Positions)
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(e => e.Position)
            .WithMany(p => p.EmployeePositions)
            .HasForeignKey(e => e.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}