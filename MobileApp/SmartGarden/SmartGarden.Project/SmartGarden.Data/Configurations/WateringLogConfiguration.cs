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
    public class WateringLogConfiguration : IEntityTypeConfiguration<WateringLog>
    {
        public void Configure(EntityTypeBuilder<WateringLog> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Mode).HasMaxLength(20).IsRequired();
            builder.Property(w => w.Timestamp).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
