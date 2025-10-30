using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.HasKey(d => d.DeviceId);

        builder.Property(d => d.DeviceName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.DeviceToken)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.MacAddress)
            .HasMaxLength(17);

        builder.Property(d => d.IpAddress)
            .HasMaxLength(45);

        builder.Property(d => d.FirmwareVersion)
            .HasMaxLength(20);

        builder.Property(d => d.Model)
            .HasMaxLength(50);

        builder.Property(d => d.SerialNumber)
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => d.PlantId);
        builder.HasIndex(d => d.DeviceToken).IsUnique();
        builder.HasIndex(d => d.MacAddress);
        builder.HasIndex(d => d.IsOnline);
        builder.HasIndex(d => d.LastSeen);

        // Relationships
        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Plant)
            .WithMany()
            .HasForeignKey(d => d.PlantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(d => d.DeviceCommands)
            .WithOne(dc => dc.Device)
            .HasForeignKey(dc => dc.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
