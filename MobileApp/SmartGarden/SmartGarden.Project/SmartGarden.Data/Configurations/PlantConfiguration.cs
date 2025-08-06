using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Configurations
{
    public class PlantConfiguration : IEntityTypeConfiguration<Plant>
    {
        public void Configure(EntityTypeBuilder<Plant> builder)
        {
            builder.HasKey(p => p.PlantId);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.MinMoisture).IsRequired();
            builder.Property(p => p.MaxMoisture).IsRequired();

            builder.HasMany(p => p.SensorReadings)
                   .WithOne(r => r.Plant)
                   .HasForeignKey(r => r.PlantId);

            builder.HasMany(p => p.WateringLogs)
                   .WithOne(w => w.Plant)
                   .HasForeignKey(w => w.PlantId);

            builder.HasOne(p => p.User)
                   .WithMany(u => u.Plants)
                   .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.Species)
                   .WithMany(s => s.Plants)
                   .HasForeignKey(p => p.SpeciesId);

            builder.HasOne(p => p.SoilType)
                   .WithMany(s => s.Plants)
                   .HasForeignKey(p => p.SoilTypeId);

        }
    }
}
