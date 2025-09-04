using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class PlantConfiguration : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> builder)
    {
        builder.HasKey(p => p.PlantId);

        builder.Property(p => p.Nickname).HasMaxLength(80);
        builder.Property(p => p.RoomName).HasMaxLength(80);

        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => new { p.UserId, p.SpeciesId });
        builder.HasIndex(p => new { p.UserId, p.IsOutdoor });

        builder.HasOne(p => p.User!)
            .WithMany(u => u.Plants)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Species!)
            .WithMany()
            .HasForeignKey(p => p.SpeciesId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.SoilType!)
            .WithMany()
            .HasForeignKey(p => p.SoilTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.SensorReadings)
            .WithOne(r => r.Plant!)
            .HasForeignKey(r => r.PlantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.WateringLogs)
            .WithOne(w => w.Plant!)
            .HasForeignKey(w => w.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
