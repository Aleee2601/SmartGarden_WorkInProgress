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
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            builder.Property(p => p.MinMoisture).IsRequired();
            builder.Property(p => p.MaxMoisture).IsRequired();

            builder.HasMany(p => p.SensorReadings)
                   .WithOne(r => r.Plant)
                   .HasForeignKey(r => r.PlantId);

            builder.HasMany(p => p.WateringLogs)
                   .WithOne(w => w.Plant)
                   .HasForeignKey(w => w.PlantId);
        }
    }
}
