using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class MaintenanceLogConfiguration : IEntityTypeConfiguration<MaintenanceLog>
{
    public void Configure(EntityTypeBuilder<MaintenanceLog> builder)
    {
        builder.HasKey(ml => ml.MaintenanceLogId);

        builder.Property(ml => ml.MaintenanceType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ml => ml.Description)
            .HasMaxLength(500);

        builder.Property(ml => ml.Cost)
            .HasColumnType("decimal(10,2)");

        // Indexes
        builder.HasIndex(ml => ml.PlantId);
        builder.HasIndex(ml => ml.PerformedAt);
        builder.HasIndex(ml => ml.NextDueAt);
        builder.HasIndex(ml => new { ml.PlantId, ml.MaintenanceType });

        // Relationships
        builder.HasOne(ml => ml.Plant)
            .WithMany()
            .HasForeignKey(ml => ml.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
