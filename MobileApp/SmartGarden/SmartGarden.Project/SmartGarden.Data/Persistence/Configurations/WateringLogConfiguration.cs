using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Configurations
{
    public class WateringLogConfiguration : IEntityTypeConfiguration<WateringLog>
    {
        public void Configure(EntityTypeBuilder<WateringLog> b)
        {
            b.HasKey(x => x.WateringId);
            b.Property(x => x.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            b.HasOne(x => x.Plant)
             .WithMany(p => p.WateringLogs)
             .HasForeignKey(x => x.PlantId)
             .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.PlantId, x.CreatedAt });
        }
    }
}
