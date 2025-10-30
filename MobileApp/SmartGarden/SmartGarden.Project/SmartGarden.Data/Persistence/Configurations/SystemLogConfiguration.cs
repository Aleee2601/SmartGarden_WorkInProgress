using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
{
    public void Configure(EntityTypeBuilder<SystemLog> builder)
    {
        builder.HasKey(sl => sl.SystemLogId);

        builder.Property(sl => sl.LogLevel)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(sl => sl.Source)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sl => sl.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(sl => sl.Endpoint)
            .HasMaxLength(255);

        builder.Property(sl => sl.Method)
            .HasMaxLength(10);

        // Indexes
        builder.HasIndex(sl => sl.LogLevel);
        builder.HasIndex(sl => sl.Timestamp);
        builder.HasIndex(sl => sl.UserId);
        builder.HasIndex(sl => new { sl.LogLevel, sl.Timestamp });

        // Relationships
        builder.HasOne(sl => sl.User)
            .WithMany()
            .HasForeignKey(sl => sl.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
