using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class PlantPhotoConfiguration : IEntityTypeConfiguration<PlantPhoto>
{
    public void Configure(EntityTypeBuilder<PlantPhoto> builder)
    {
        builder.HasKey(pp => pp.PlantPhotoId);

        builder.Property(pp => pp.PhotoUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(pp => pp.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(pp => pp.Caption)
            .HasMaxLength(500);

        builder.Property(pp => pp.Format)
            .HasMaxLength(10);

        // Indexes
        builder.HasIndex(pp => pp.PlantId);
        builder.HasIndex(pp => pp.TakenAt);
        builder.HasIndex(pp => new { pp.PlantId, pp.TakenAt });

        // Relationships
        builder.HasOne(pp => pp.Plant)
            .WithMany()
            .HasForeignKey(pp => pp.PlantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
