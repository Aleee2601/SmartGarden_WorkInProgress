using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartGarden.Core.Models;

namespace SmartGarden.Data.Persistence.Configurations;

public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
{
    public void Configure(EntityTypeBuilder<UserSetting> builder)
    {
        builder.HasKey(s => s.UserId); // PK = FK

        builder.Property(s => s.AutoWateringEnabled)
            .HasDefaultValue(false);

        builder.Property(s => s.SoilMoistThreshold)
            .HasDefaultValue(30.0); // %

        builder.Property(s => s.DataReadIntervalMin)
            .HasDefaultValue(15); // minute
    }
}
