using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class SoilTypeConfiguration : IEntityTypeConfiguration<SoilType>
{
    public void Configure(EntityTypeBuilder<SoilType> builder)
    {
        builder.HasKey(s => s.SoilTypeId);

        builder.Property(s => s.Name)
            .IsRequired().HasMaxLength(80);

        builder.Property(s => s.Description)
            .HasMaxLength(400);

        // recomandări udare
        builder.Property(s => s.RecWaterDueSec).HasDefaultValue(5);
        builder.Property(s => s.PauseBetweenWaterMin).HasDefaultValue(2);

        // Seed minim
        builder.HasData(
            new SoilType
            {
                SoilTypeId = 1,
                Name = "Universal (pământ flori)",
                Description = "Mix general pentru plante de interior",
                RecWaterDueSec = 5,
                PauseBetweenWaterMin = 2
            },
            new SoilType
            {
                SoilTypeId = 2,
                Name = "Cactuși/Suculente (nisipos)",
                Description = "Drenaj rapid, retentie mică",
                RecWaterDueSec = 3,
                PauseBetweenWaterMin = 5
            }
        );
    }
}
