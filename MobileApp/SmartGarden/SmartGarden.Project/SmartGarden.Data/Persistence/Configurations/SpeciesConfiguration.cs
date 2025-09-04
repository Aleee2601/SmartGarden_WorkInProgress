using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
{
    public void Configure(EntityTypeBuilder<Species> builder)
    {
        builder.HasKey(s => s.SpeciesId);

        builder.Property(s => s.CommonName)
            .IsRequired().HasMaxLength(120);

        builder.Property(s => s.ScientificName)
            .IsRequired().HasMaxLength(160);

        builder.HasIndex(s => s.ScientificName).IsUnique();

        // Seed minim (poți extinde sau importa din CSV separat)
        builder.HasData(
            new Species
            {
                SpeciesId = 1,
                CommonName = "Ficus lyrata",
                ScientificName = "Ficus lyrata",
                DefaultSoilMoistMin = 30,
                DefaultSoilMoistMax = 60,
                DefaultTempMin = 18,
                DefaultTempMax = 27,
                DefaultLightMin = 500,
                DefaultLightMax = 10000,
                DefaultHumidityMin = 30,
                DefaultHumidityMax = 70
            },
            new Species
            {
                SpeciesId = 2,
                CommonName = "Monstera deliciosa",
                ScientificName = "Monstera deliciosa",
                DefaultSoilMoistMin = 30,
                DefaultSoilMoistMax = 65,
                DefaultTempMin = 18,
                DefaultTempMax = 28,
                DefaultLightMin = 300,
                DefaultLightMax = 8000,
                DefaultHumidityMin = 40,
                DefaultHumidityMax = 80
            }
        );
    }
}
