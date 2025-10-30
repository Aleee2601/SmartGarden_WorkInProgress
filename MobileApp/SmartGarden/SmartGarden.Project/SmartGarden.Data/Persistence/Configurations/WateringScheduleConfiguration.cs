using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class WateringScheduleConfiguration : IEntityTypeConfiguration<WateringSchedule>
{
    public void Configure(EntityTypeBuilder<WateringSchedule> builder)
    {
        builder.HasKey(ws => ws.WateringScheduleId);

        builder.Property(ws => ws.ScheduleName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ws => ws.ScheduleType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ws => ws.DaysOfWeek)
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(ws => ws.PlantId);
        builder.HasIndex(ws => ws.IsActive);
        builder.HasIndex(ws => ws.NextRunAt);
        builder.HasIndex(ws => new { ws.PlantId, ws.IsActive });

        // Relationships
        builder.HasOne(ws => ws.Plant)
            .WithMany()
            .HasForeignKey(ws => ws.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
