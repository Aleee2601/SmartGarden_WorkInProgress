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
    public class UserSettingConfiguration : IEntityTypeConfiguration<UserSetting>
    {
        public void Configure(EntityTypeBuilder<UserSetting> builder)
        {
            builder.HasKey(s => s.UserId);
            builder.Property(s => s.SoilMoistThreshold).HasDefaultValue(30.0);
            builder.Property(s => s.DataReadIntervalMin).HasDefaultValue(15);
        }
    }
}
