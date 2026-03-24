using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnboardingSIGDB1.Domain.Entities;

namespace OnboardingSIGDB1.Data.Mappings;

public class PositionMap : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        builder.ToTable("Positions");
        
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(250);
        
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.HasMany(p => p.EmployeePositions)
            .WithOne(p => p.Position)
            .HasForeignKey(f => f.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Navigation(p => p.EmployeePositions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}