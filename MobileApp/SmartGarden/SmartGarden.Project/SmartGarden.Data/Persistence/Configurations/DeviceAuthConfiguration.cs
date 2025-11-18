using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations
{
    public class DeviceAuthConfiguration : IEntityTypeConfiguration<DeviceAuth>
    {
        public void Configure(EntityTypeBuilder<DeviceAuth> builder)
        {
            builder.ToTable("DeviceAuths");

            builder.HasKey(da => da.DeviceAuthId);

            // Indexes for performance
            builder.HasIndex(da => da.DeviceId).IsUnique();
            builder.HasIndex(da => da.IsApproved);
            builder.HasIndex(da => da.IsLocked);
            builder.HasIndex(da => da.LastRequestAt);

            // Required fields
            builder.Property(da => da.ApiKeyHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(da => da.RefreshToken)
                .IsRequired()
                .HasMaxLength(500);

            // Optional fields with limits
            builder.Property(da => da.CertificateFingerprint)
                .HasMaxLength(256);

            builder.Property(da => da.ApprovedByUserId)
                .HasMaxLength(450);

            // Default values
            builder.Property(da => da.IsApproved)
                .HasDefaultValue(false);

            builder.Property(da => da.IsLocked)
                .HasDefaultValue(false);

            builder.Property(da => da.RequestCount)
                .HasDefaultValue(0);

            builder.Property(da => da.FailedAuthAttempts)
                .HasDefaultValue(0);

            // Relationships
            builder.HasOne(da => da.Device)
                .WithOne(d => d.DeviceAuth)
                .HasForeignKey<DeviceAuth>(da => da.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
