using Microsoft.EntityFrameworkCore;
using SmartGarden.API.Models;

namespace SmartGarden.API.Data
{
    public class SmartGardenContext : DbContext
    {
        public SmartGardenContext(DbContextOptions<SmartGardenContext> options) : base(options) { }

        public DbSet<Plant> Plants { get; set; }
        public DbSet<IrrigationEvent> IrrigationEvents { get; set; }
        public DbSet<SensorReading> SensorReadings { get; set; }
        public DbSet<PlantType> PlantTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AlertEvent> AlertEvents { get; set; }

    }
}
