using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(al => al.AuditLogId);

        builder.Property(al => al.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(al => al.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(al => al.IpAddress)
            .HasMaxLength(45);

        builder.Property(al => al.UserAgent)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(al => al.UserId);
        builder.HasIndex(al => al.EntityType);
        builder.HasIndex(al => al.EntityId);
        builder.HasIndex(al => al.Timestamp);
        builder.HasIndex(al => new { al.EntityType, al.EntityId });
        builder.HasIndex(al => new { al.UserId, al.Timestamp });

        // Relationships
        builder.HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
