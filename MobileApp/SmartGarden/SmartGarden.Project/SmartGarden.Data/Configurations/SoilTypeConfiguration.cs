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
    public class SoilTypeConfiguration : IEntityTypeConfiguration<SoilType>
    {
        public void Configure(EntityTypeBuilder<SoilType> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
        }
    }
}
