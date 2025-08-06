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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasMany(u => u.Plants)
                   .WithOne(p => p.User)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.UserSetting)
                   .WithOne(s => s.User)
                   .HasForeignKey<UserSetting>(s => s.UserId);
        }
    }
}
