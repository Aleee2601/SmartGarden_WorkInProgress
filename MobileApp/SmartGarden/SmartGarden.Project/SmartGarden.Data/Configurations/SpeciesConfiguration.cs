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
    public class SpeciesConfiguration : IEntityTypeConfiguration<Species>
    {
        public void Configure(EntityTypeBuilder<Species> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.CommonName).IsRequired().HasMaxLength(100);
            builder.Property(s => s.DefaultSoilMoistMin).IsRequired();
            builder.Property(s => s.DefaultSoilMoistMax).IsRequired();
        }
    }
}
