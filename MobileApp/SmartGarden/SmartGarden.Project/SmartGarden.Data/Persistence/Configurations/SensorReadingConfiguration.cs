using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class SensorReadingConfiguration : IEntityTypeConfiguration<SensorReading>
{
    public void Configure(EntityTypeBuilder<SensorReading> builder)
    {
        builder.HasKey(r => r.ReadingId);

        // Index compus pentru interogări latest/history
        builder.HasIndex("PlantId", "CreatedAt");
        builder.HasIndex(r => r.PlantId);

        // Numericele le lăsăm double/int fără precizie fixă (portabil între SQLite/SQL Server)
        // Dacă la tine sunt decimal, poți seta HasPrecision(...)

        // CreatedAt shadow prop are default în DbContext, aici doar îl indexăm.
    }
}
