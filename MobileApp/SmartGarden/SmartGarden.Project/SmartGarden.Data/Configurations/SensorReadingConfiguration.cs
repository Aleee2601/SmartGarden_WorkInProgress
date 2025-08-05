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
    public class SensorReadingConfiguration : IEntityTypeConfiguration<SensorReading>
    {
        public void Configure(EntityTypeBuilder<SensorReading> builder)
        {
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Temperature).IsRequired();
            builder.Property(r => r.Humidity).IsRequired();
            builder.Property(r => r.SoilMoisture).IsRequired();
            builder.Property(r => r.Timestamp).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
