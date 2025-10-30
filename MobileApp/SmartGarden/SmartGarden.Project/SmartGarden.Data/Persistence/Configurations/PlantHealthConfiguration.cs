using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class PlantHealthConfiguration : IEntityTypeConfiguration<PlantHealth>
{
    public void Configure(EntityTypeBuilder<PlantHealth> builder)
    {
        builder.HasKey(ph => ph.PlantHealthId);

        builder.Property(ph => ph.HealthStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ph => ph.Diagnosis)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(ph => ph.PlantId);
        builder.HasIndex(ph => ph.CalculatedAt);
        builder.HasIndex(ph => new { ph.PlantId, ph.CalculatedAt });
        builder.HasIndex(ph => ph.HealthStatus);

        // Relationships
        builder.HasOne(ph => ph.Plant)
            .WithMany()
            .HasForeignKey(ph => ph.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
