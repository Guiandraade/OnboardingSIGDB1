using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnboardingSIGDB1.Domain.Entities.Positions;

namespace OnboardingSIGDB1.Data.Mappings;

public class PositionMap : IEntityTypeConfiguration<Position>
{
    public void Configure(EntityTypeBuilder<Position> builder)
    {
        // Table & Primary Key
        builder.ToTable("Positions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();

        // Properties
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(p => p.CreatedAt).IsRequired();

        // Ignored properties
        builder.Ignore(p => p.ValidationResult);

        // Relationships
        builder.HasMany(p => p.EmployeePositions)
            .WithOne(ep => ep.Position)
            .HasForeignKey(ep => ep.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Navigation settings
        builder.Navigation(p => p.EmployeePositions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}