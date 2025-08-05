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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SmartGardenDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
