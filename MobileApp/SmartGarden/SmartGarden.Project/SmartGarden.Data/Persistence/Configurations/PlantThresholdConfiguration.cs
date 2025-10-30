using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class PlantThresholdConfiguration : IEntityTypeConfiguration<PlantThreshold>
{
    public void Configure(EntityTypeBuilder<PlantThreshold> builder)
    {
        builder.HasKey(pt => pt.PlantThresholdId);

        // Indexes
        builder.HasIndex(pt => pt.PlantId);
        builder.HasIndex(pt => new { pt.PlantId, pt.IsActive });

        // Relationships
        builder.HasOne(pt => pt.Plant)
            .WithMany()
            .HasForeignKey(pt => pt.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
