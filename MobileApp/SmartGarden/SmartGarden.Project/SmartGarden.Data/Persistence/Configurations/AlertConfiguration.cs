using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.HasKey(a => a.AlertId);

        builder.Property(a => a.AlertType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Severity)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Message)
            .HasMaxLength(1000);

        // Indexes
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.PlantId);
        builder.HasIndex(a => a.DeviceId);
        builder.HasIndex(a => a.TriggeredAt);
        builder.HasIndex(a => a.IsRead);
        builder.HasIndex(a => a.IsResolved);
        builder.HasIndex(a => new { a.UserId, a.IsRead });
        builder.HasIndex(a => new { a.UserId, a.Severity, a.IsResolved });

        // Relationships
        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Plant)
            .WithMany()
            .HasForeignKey(a => a.PlantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Device)
            .WithMany()
            .HasForeignKey(a => a.DeviceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
