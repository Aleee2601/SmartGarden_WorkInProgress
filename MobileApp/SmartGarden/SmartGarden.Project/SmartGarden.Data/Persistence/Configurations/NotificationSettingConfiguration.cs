using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class NotificationSettingConfiguration : IEntityTypeConfiguration<NotificationSetting>
{
    public void Configure(EntityTypeBuilder<NotificationSetting> builder)
    {
        builder.HasKey(ns => ns.NotificationSettingId);

        builder.Property(ns => ns.Channel)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(ns => ns.AlertTypeFilter)
            .HasMaxLength(500);

        builder.Property(ns => ns.Endpoint)
            .HasMaxLength(255);

        // Indexes
        builder.HasIndex(ns => ns.UserId);
        builder.HasIndex(ns => new { ns.UserId, ns.Channel }).IsUnique();
        builder.HasIndex(ns => ns.IsEnabled);

        // Relationships
        builder.HasOne(ns => ns.User)
            .WithMany()
            .HasForeignKey(ns => ns.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
