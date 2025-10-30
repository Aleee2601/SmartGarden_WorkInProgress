using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class DeviceCommandConfiguration : IEntityTypeConfiguration<DeviceCommand>
{
    public void Configure(EntityTypeBuilder<DeviceCommand> builder)
    {
        builder.HasKey(dc => dc.DeviceCommandId);

        builder.Property(dc => dc.CommandType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(dc => dc.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(dc => dc.FailReason)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(dc => dc.DeviceId);
        builder.HasIndex(dc => dc.Status);
        builder.HasIndex(dc => dc.ScheduledAt);
        builder.HasIndex(dc => new { dc.DeviceId, dc.Status });

        // Relationships
        builder.HasOne(dc => dc.Device)
            .WithMany(d => d.DeviceCommands)
            .HasForeignKey(dc => dc.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
