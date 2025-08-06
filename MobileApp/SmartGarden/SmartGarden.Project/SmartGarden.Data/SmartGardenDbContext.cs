using Microsoft.EntityFrameworkCore;
using SmartGarden.Core.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SmartGarden.Data
{
    public class SmartGardenDbContext : DbContext
    {
        public SmartGardenDbContext(DbContextOptions<SmartGardenDbContext> options)
            : base(options) { }

        public DbSet<Plant> Plants => Set<Plant>();
        public DbSet<SensorReading> SensorReadings => Set<SensorReading>();
        public DbSet<WateringLog> WateringLogs => Set<WateringLog>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Species> Species => Set<Species>();
        public DbSet<SoilType> SoilTypes => Set<SoilType>();
        public DbSet<UserSetting> UserSettings => Set<UserSetting>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartGardenDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
